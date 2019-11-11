#pragma once
#include "pcap.h"
#include "ipheader.h"
#pragma comment(lib,"wpcap.lib")
#pragma comment(lib,"ws2_32.lib")

using namespace System;
using namespace System::Runtime::InteropServices;
using namespace System::Threading;

public ref struct pcap_if_c
{
	IntPtr next;
	String^   cname;
	String^   cdescription;
	IntPtr addresses;
	void UnmanagedPtr2ManagedStru(IntPtr ptr)
	{
		pcap_if *ppcap_if = static_cast<pcap_if *>(ptr.ToPointer());
		if (NULL == ppcap_if)
			return;
		next = (IntPtr)ppcap_if->next;
		addresses = (IntPtr)ppcap_if->addresses;
		cname = Marshal::PtrToStringAnsi(static_cast<IntPtr>(ppcap_if->name));
		cdescription = Marshal::PtrToStringAnsi(static_cast<IntPtr>(ppcap_if->description));
	}
};

public ref struct pcap_data
{
	int res;
	int len;
	String^  time;
	String^ source_ip;
	String^ dest_ip;
	u_short source_port;
	u_short dest_port;
	u_char	ttl;
	u_char type;
	IntPtr p_data;
};

namespace clrtool {
	public ref class mytool
	{
		// TODO: 在此处为此类添加方法。
	private:

	public:
		pcap_t *adhandle;
		IntPtr alldevs_lode();
		void dev_free(IntPtr ptr);
		bool open(String^ name, IntPtr ptr, String ^ filter);
		String^ getlpv4(IntPtr ptr);
		IntPtr getnetmask(IntPtr ptr);

		GPacket::Result parse_(byte* cap_data, PIpHdr& c_iphdr, PTcpHdr&tcpHdr_, PUdpHdr&c_udpHdr_, GBuf& c_Data_);
		pcap_data^ read();
	};
}
