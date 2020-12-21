using NetCon.export.entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace NetCon.export.services
{
    class EntityPersistor
    {
        private static EntityPersistor instance;
        public static EntityPersistor GetInstance()
        {
            if (instance == null)
                instance = new EntityPersistor();
            return instance;
        }

        private EntityPersistor() { }

        public string ConnectionString { get; set; }

        private const int TARGET_BUFFER_MAX_SIZE = 1024;
        private const int TRANSACTION_INTERVAL = 500;
        private const int WAIT_FOR_QUEUE_PERIOD = 10;

        private EntityMapper entityMapper = EntityMapper.GetInstance();

        private List<Target> targetBuffer = new List<Target>(TARGET_BUFFER_MAX_SIZE);
        private ConcurrentQueue<List<Target>> targetFIFO = new ConcurrentQueue<List<Target>>();

        private Stopwatch stopwatch = new Stopwatch();
        private bool isRunning = false;

        public void StartExporting()
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
                throw new ArgumentException("ConnectionString must be initialized for EntityPersistor before starting to export!");
            if (isRunning)
                throw new InvalidOperationException("Attempted to start exporting while exporting already in progress!");

            stopwatch.Restart();
            isRunning = true;

            Task.Run(() =>
            {
                List<Target> popped = new List<Target>();
                while (isRunning)
                {
                    if (targetFIFO.TryDequeue(out popped))
                    {
                        persistTargetBuffer(popped);
                        Console.WriteLine(String.Format("{1} persisted {0} Targets.", popped.Count, DateTime.Now.ToString()));
                    }
                    else
                    {
                        Console.WriteLine(String.Format("Queue empty, waiting for {0} miliseconds...", WAIT_FOR_QUEUE_PERIOD));
                        Thread.Sleep(WAIT_FOR_QUEUE_PERIOD);
                    }
                }

                // Empty the queue before ending:
                while (targetFIFO.TryDequeue(out popped))
                {
                    persistTargetBuffer(popped);
                    Console.WriteLine(String.Format("{1} persisted {0} Targets.", popped.Count, DateTime.Now.ToString()));
                }
            });
        }

        public void StopExporting()
        {
            List<Target> targetBufferShallowCopy = new List<Target>(targetBuffer);
            targetFIFO.Enqueue(targetBufferShallowCopy);
            targetBuffer.Clear();
            isRunning = false;
            stopwatch.Stop();
        }

        public void BufferTargetData(filtering.TargetDataDto targetDataDto)
        {
            if (string.IsNullOrWhiteSpace(ConnectionString))
                throw new ArgumentException("ConnectionString has not been initialized for EntityPersistor");

            Target target = entityMapper.ToNewTarget(targetDataDto);
            target.SetTargetName(entityMapper.ToNewTargetName(targetDataDto));
            if (targetDataDto.RegisterChanges && targetDataDto.TriggeredThreshold)
                target.SetVariableEvent(entityMapper.ToNewVariableEvent(targetDataDto));

            targetBuffer.Add(target);

            if (isRunning && ((stopwatch.ElapsedMilliseconds > TRANSACTION_INTERVAL) || (targetBuffer.Count > TARGET_BUFFER_MAX_SIZE)))
            {
                List<Target> targetBufferShallowCopy = new List<Target>(targetBuffer);
                targetFIFO.Enqueue(targetBufferShallowCopy);
                targetBuffer.Clear();
                stopwatch.Restart();
            }
        }

        private void persistTargetBuffer(List<Target> targetBuffer)
        {
            using (DBContext dBContext = new DBContext(ConnectionString))
            {
                try
                {
                    targetBuffer.ForEach(target =>
                    {
                        TargetName targetName = dBContext.TargetNames.Find(target.TargetNameID);
                        if (targetName != null)
                        {
                            if (!targetName.Name.Equals(target.TargetName.Name))
                            {
                                targetName.Name = target.TargetName.Name;
                                targetName.ModifyDate = target.Date;
                            }
                            target.SetTargetName(targetName);
                        }
                        dBContext.Targets.Add(target);
                    });
                    dBContext.SaveChanges();
                }
                catch (SqlException e)
                {
                    // TODO: Better exception handling?
                    Console.WriteLine("ERROR PERSISTING TARGET BUFFER!");
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    StopExporting();
                    targetFIFO = new ConcurrentQueue<List<Target>>();
                }
            }
        }

    }
}
