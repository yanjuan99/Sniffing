#include "stdafx.h"
#include "clrtool.h"

IntPtr clrtool::mytool::alldevs_lode()
{
	pcap_if *dev;
	char errbuf[PCAP_ERRBUF_SIZE]{ 0 };

	if (pcap_findalldevs_ex((char*)PCAP_SRC_IF_STRING, NULL, &dev, errbuf) == -1)
	{
		return (IntPtr)NULL;
	}
	return (IntPtr)dev;
}

void clrtool::mytool::dev_free(IntPtr ptr)
{
	pcap_freealldevs(static_cast<pcap_if_t *>(ptr.ToPointer()));
}
  
bool clrtool::mytool::open(String ^ name, IntPtr ptr, String ^ filter)
{
	char errbuf[PCAP_ERRBUF_SIZE]{ 0 };
	char* namestr = (char*)(void*)Marshal::StringToHGlobalAnsi(name);
	adhandle = pcap_open(namestr, 65535, PCAP_OPENFLAG_PROMISCUOUS, 1000, NULL, errbuf);
	//printf(namestr);
	Marshal::FreeHGlobal(IntPtr(namestr));

	if (adhandle == NULL)
	{
		return false;
	}

	/* 检查数据链路层，为了简单，我们只考虑以太网 */
	if (pcap_datalink(adhandle) != DLT_EN10MB)
	{
		return false;
	}

	//u_int netmask= 0xffffff;
	//pcap_addr * p_addr = (pcap_addr *)ptr.ToPointer();
	//netmask = p_addr != NULL ? netmask = ((struct sockaddr_in *)(p_addr->netmask))->sin_addr.S_un.S_addr : netmask = 0xffffff;


	struct bpf_program fcode;
	char* filterstr = (char*)(void*)Marshal::StringToHGlobalAnsi(filter);
	//printf(filterstr);

	//编译过滤器
	if (pcap_compile(adhandle, &fcode, filterstr, 1, ptr.ToInt32()) < 0)
	{
		Marshal::FreeHGlobal(IntPtr(filterstr));
		return false;
	}
	Marshal::FreeHGlobal(IntPtr(filterstr));

	//设置过滤器
	if (pcap_setfilter(adhandle, &fcode) < 0)
	{
		return false;
	}

	return true;
}

String ^ clrtool::mytool::getlpv4(IntPtr ptr)
{
	for (auto a = static_cast<pcap_addr *>(ptr.ToPointer()); a; a = a->next)
	{
		if (a->addr->sa_family == AF_INET)
		{
			char buf[INET_ADDRSTRLEN]{ 0 };
			inet_ntop(AF_INET, &(((struct sockaddr_in *)a->addr)->sin_addr), buf, sizeof(buf));
			return Marshal::PtrToStringAnsi(static_cast<IntPtr>(buf));
		}
	}
	return "";
}

IntPtr clrtool::mytool::getnetmask(IntPtr ptr)
{
	int netmask = 0;
	pcap_addr * p_addr = (pcap_addr *)ptr.ToPointer();
	if (p_addr != NULL)
		/* 获取接口第一个地址的掩码 */
		netmask = ((struct sockaddr_in *)(p_addr->netmask))->sin_addr.S_un.S_addr;
	else
		/* 如果这个接口没有地址，那么我们假设这个接口在C类网络中 */
		netmask = 0xffffff;
	return (IntPtr)netmask;
}
 

GPacket::Result clrtool::mytool::parse_(byte* cap_data, PIpHdr& c_iphdr, PTcpHdr&c_tcpHdr_, PUdpHdr&c_udpHdr_, GBuf& c_Data_)
{
	byte* p = cap_data + sizeof(GEthHdr);
	c_iphdr = PIpHdr(p);
	if (c_iphdr->v == 4)
	{
		p += c_iphdr->hl * 4;
		switch (c_iphdr->p_)
		{
		case GIpHdr::Tcp:
			c_tcpHdr_ = PTcpHdr(p);
			p += c_tcpHdr_->off() * 4;
			c_Data_ = GTcpHdr::parseData(c_iphdr, c_tcpHdr_);
			break;
		case GIpHdr::Udp:
			c_udpHdr_ = PUdpHdr(p);
			c_Data_ = GUdpHdr::parseData(c_udpHdr_);
			break;
		default:
			break;
		}
		return c_Data_.size_ > 0 ? GPacket::Ok : GPacket::TimeOut;
	}
	return  GPacket::Eof;
}

pcap_data^ clrtool::mytool::read()
{
	pcap_data^ result = gcnew pcap_data();
	struct pcap_pkthdr *header;
	const u_char *pkt_data;
	int i = pcap_next_ex(adhandle, &header, &pkt_data);
	GPacket::Result res;

	GIpHdr* iphdr = nullptr;
	GTcpHdr* tcpHdr_ = nullptr;
	GUdpHdr* udpHdr_ = nullptr;
	GBuf Data_;
	switch (i)
	{
	case -2:
		res = GPacket::Eof; break;
	case -1:
		res = GPacket::Eof;  break;
	case 0:
		res = GPacket::TimeOut;  break;
	default:
		res = parse_(const_cast<byte*>(pkt_data), iphdr, tcpHdr_, udpHdr_, Data_);
		break;
	}
	if (  res == GPacket::TimeOut || Data_.size_ <= 0)
	{
		result->res = 0; return result;
	}
	if (res == GPacket::Eof)
	{
		result->res = -1; return result;
	}

	//printf("%d", header->len);
	time_t local_tv_sec; struct tm *ltime=nullptr;	char timestr[16];
	local_tv_sec = header->ts.tv_sec;
	ltime=localtime(&local_tv_sec);
	strftime(timestr, sizeof timestr, "%H:%M:%S", ltime);
	result->time = Marshal::PtrToStringAnsi(static_cast<IntPtr>(timestr));

	result->ttl = iphdr->ttl_;
	result->type = iphdr->p_;

	result->p_data = (IntPtr)Data_.data_; 
	result->len = Data_.size_;
	

	char buf[INET_ADDRSTRLEN]{ 0 };
	inet_ntop(AF_INET, &(iphdr->sip_), buf, sizeof(buf));
	result->source_ip = Marshal::PtrToStringAnsi(static_cast<IntPtr>(buf));

	inet_ntop(AF_INET, &(iphdr->dip_), buf, sizeof(buf));
	result->dest_ip = Marshal::PtrToStringAnsi(static_cast<IntPtr>(buf));

	result->source_port = iphdr->p_ == GIpHdr::Tcp ? tcpHdr_->sport() : udpHdr_->sport();
	result->dest_port = iphdr->p_ == GIpHdr::Tcp ? tcpHdr_->dport() : udpHdr_->dport();
	result->res = 1;
	return result;
}
 