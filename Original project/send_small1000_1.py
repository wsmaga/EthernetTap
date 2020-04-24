from scapy.all import *

sendp("abcdefghijklmnopqrstuvwxyz", iface=ifaces.dev_from_pcapname('\\Device\\NPF_{DB07413B-64B8-4005-A7B3-9F13EAB5210E}'), count=1000)