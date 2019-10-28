#pragma once
#include <stdint.h>
#include <windows.h>

// ----------------------------------------------------------------------------
// GEthHdr
// ----------------------------------------------------------------------------
#pragma pack(push, 1)
struct  GEthHdr  {
	uint8_t     dmac_[6];
	uint8_t     smac_[6];
	uint16_t type_;

	uint8_t*     dmac() { return dmac_; }
	uint8_t*     smac() { return smac_; }
	uint16_t type() { return ntohs(type_); }

	typedef enum {
		Ip4 = 0x0800,
		Arp = 0x0806,
		Ip6 = 0x86DD
	} Type;
};
typedef GEthHdr *PEthHdr;
#pragma pack(pop)

#pragma pack(push, 1)
struct GIpHdr  {
	uint8_t  hl :4;
	uint8_t  v:4;
	uint8_t  tos_;
	uint16_t len_;
	uint16_t id_;
	uint16_t off_;
	uint8_t  ttl_;
	uint8_t  p_;
	uint16_t sum_;
	char      sip_[4];
	char      dip_[4];

	uint8_t  tos() { return tos_; }
	uint16_t len() { return ntohs(len_); }
	uint16_t id() { return ntohs(id_); }
	uint16_t off() { return ntohs(off_); }
	uint8_t  ttl() { return ttl_; }
	//uint8_t  p() { return p_; }
	uint16_t sum() { return ntohs(sum_); }
	
	enum Protocol {
		Icmp = 1,   // Internet Control Message Protocol
		Igmp = 2,   // Internet Group Management Protocol
		Tcp = 6,   // Transmission Control Protocol
		Udp = 17,  // User Datagram Protocol
		Sctp = 132, // Stream Control Transport Protocol
	};
};
typedef GIpHdr *PIpHdr;
#pragma pack(pop)

// ----------------------------------------------------------------------------
// GBuf
// ----------------------------------------------------------------------------
struct GBuf  {
	unsigned char* data_; // u_char*, gbyte*
	size_t size_;

	GBuf() {}
	GBuf(unsigned char* data, size_t size) : data_(data), size_(size) {}

	void clear() {
		data_ = nullptr;
		size_ = 0;
	}

	bool valid() {
		return data_ != nullptr;
	}
};

// ----------------------------------------------------------------------------
// GUdpHdr
// ----------------------------------------------------------------------------
#pragma pack(push, 1)
struct GUdpHdr  { // libnet_tcp_hdr // gilgil temp 2019.05.13
	uint16_t sport_;
	uint16_t dport_;
	uint16_t len_;
	uint16_t sum_;

	uint16_t sport() { return ntohs(sport_); }
	uint16_t dport() { return ntohs(dport_); }
	uint16_t len() { return ntohs(len_); }
	uint16_t sum() { return ntohs(sum_); }


	static GBuf parseData(GUdpHdr* udpHdr);
};
typedef GUdpHdr *PUdpHdr;

#pragma pack(pop)
GBuf GUdpHdr::parseData(GUdpHdr* udpHdr) {
	GBuf res;
	res.size_ = udpHdr->len() - sizeof(GUdpHdr);
	if (res.size_ > 0)
		res.data_ = reinterpret_cast<u_char*>(udpHdr) + sizeof(GUdpHdr);
	else
		res.data_ = nullptr;
	return res;
}

// ----------------------------------------------------------------------------
// GTcpHdr
// ----------------------------------------------------------------------------
#pragma pack(push, 1)
struct GTcpHdr  {
	uint16_t sport_;
	uint16_t dport_;
	uint32_t seq_;
	uint32_t ack_;
	uint8_t  off_rsvd_;
	uint8_t  flags_;
	uint16_t win_;
	uint16_t sum_;
	uint16_t urp_;

	uint16_t sport() { return ntohs(sport_); }
	uint16_t dport() { return ntohs(dport_); }
	uint32_t seq() { return ntohl(seq_); }
	uint32_t ack() { return ntohl(ack_); }
	uint8_t  off() { return (off_rsvd_ & 0xF0) >> 4; }
	uint8_t  rsvd() { return off_rsvd_ & 0x0F; }
	uint8_t  flags() { return flags_; }
	uint16_t win() { return ntohs(win_); }
	uint16_t sum() { return ntohs(sum_); }
	uint16_t urp() { return ntohs(urp_); }

	enum Flag {
		Urg = 0x20,
		Ack = 0x10,
		Psh = 0x08,
		Rst = 0x04,
		Syn = 0x02,
		Fin = 0x01
	};


	static GBuf parseData(GIpHdr* ipHdr, GTcpHdr* tcpHdr);
};
typedef GTcpHdr *PTcpHdr;
#pragma pack(pop)


GBuf GTcpHdr::parseData(GIpHdr* ipHdr, GTcpHdr* tcpHdr) {
	GBuf res;
	res.size_ = ipHdr->len() - ipHdr->hl * 4 - tcpHdr->off() * 4;
	if (res.size_ > 0)
		res.data_ = reinterpret_cast<u_char*>(tcpHdr) + tcpHdr->off() * 4;
	else
		res.data_ = nullptr;
	return res;
}

// ----------------------------------------------------------------------------
// GPacket
// ----------------------------------------------------------------------------
struct  GPacket  {
public:
	// --------------------------------------------------------------------------
	// Result
	// --------------------------------------------------------------------------
	typedef enum {
		Eof = -2, // read
		Fail = -1, // read write
		TimeOut = 0,  // read
		Ok = 1,  // read write
	} Result;

	// --------------------------------------------------------------------------
	// DataLinkType
	// --------------------------------------------------------------------------
	typedef enum {
		Eth,   // DLT_EN10MB (1)
		Ip,    // DLT_RAW (228)
		Dot11, // DLT_IEEE802_11_RADIO (127)
		Null,  // DLT_NULL (0)
	} DataLinkType;
public:
public:
	//
	// info
	//
	DataLinkType dataLinkType_{ Null };

	//
	// sniffing
	//
	struct timeval ts_;
	GBuf buf_;
	bool bufSelfAlloc_{ false };

	//
	// control
	//
	struct {
		bool block_{ false };
		bool changed_{ false };
	} ctrl;

	//
	// header
	//
	GEthHdr* ethHdr_{ nullptr };
	//GArpHdr* arpHdr_{ nullptr };

	GIpHdr* ipHdr_{ nullptr };
	//GIp6Hdr* ip6Hdr_{ nullptr };

	GTcpHdr* tcpHdr_{ nullptr };
	GUdpHdr* udpHdr_{ nullptr };
	//GIcmpHdr* icmpHdr_{ nullptr };

	GBuf tcpData_;
	GBuf udpData_;

	//
	// user
	//
	static const int USER_DATA_SIZE = 256;
	byte userData_[USER_DATA_SIZE];

#ifdef _DEBUG
	//
	// debug
	//
	bool parsed_;
#endif // _DEBUG

public:
	void clear() {
		ts_.tv_sec = 0;
		ts_.tv_usec = 0;
		if (!bufSelfAlloc_)
			buf_.clear();
		ctrl.block_ = false;
		ctrl.changed_ = false;
		ethHdr_ = nullptr;
		//arpHdr_ = nullptr;
		ipHdr_ = nullptr;
		//ip6Hdr_ = nullptr;
		tcpHdr_ = nullptr;
		udpHdr_ = nullptr;
		//icmpHdr_ = nullptr;
		tcpData_.clear();
		udpData_.clear();
#ifdef _DEBUG
		parsed_ = false;
#endif // _DEBUG
	}

	void parse();
};
typedef GPacket *PPacket;

// ----------------------------------------------------------------------------
// GIpPacket
// ----------------------------------------------------------------------------
void GPacket::parse() {
#ifdef _DEBUG
	if (parsed_) {
		//qCritical() << "already parsed";
		return;
	}
#endif // _DEBUG
	byte* p = buf_.data_+sizeof(GEthHdr);
	uint8_t proto;
	switch (*p & 0xF0) {
	case 0x40: // version 4
		ipHdr_ = PIpHdr(p);
		proto = ipHdr_->p_;
		p += ipHdr_->hl * 4;
		break;
	//case 0x60: // version 6
	//	ip6Hdr_ = PIp6Hdr(p);
	//	proto = ip6Hdr_->nh();
	//	p += sizeof(GIp6Hdr); // gilgil temp 2019.05.14
	//	break;
	//default:
	//	qWarning() << "invalid ip header version" << uint8_t(*p); // gilgil temp 2019.05.31
	//	proto = 0; // unknown
	//	break;
	}

	switch (proto) {
	case GIpHdr::Tcp: // Tcp
		tcpHdr_ = PTcpHdr(p);
		p += tcpHdr_->off() * 4;
		tcpData_ = GTcpHdr::parseData(ipHdr_, tcpHdr_);
		break;
	case GIpHdr::Udp: // Udp
		udpHdr_ = PUdpHdr(p);
		p += sizeof(GUdpHdr);
		udpData_ = GUdpHdr::parseData(udpHdr_);
		break;
	//case GIpHdr::Icmp: // Icmp
	//	icmpHdr_ = PIcmpHdr(p);
	//	p += sizeof(GIcmpHdr);
	//	break;
	default:
		// qDebug() << "unknown protocol" << proto; // gilgil temp 2019.08.19
		break;
	}
#ifdef _DEBUG
	parsed_ = true;
#endif // _DEBUG
}