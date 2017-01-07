# vyatta-config-updater
A tool to inject new rules into a Vyatta config (for use with EdgeRouter/EdgeMax/EdgeOS devices etc.)

**This tool is not yet ready for actual use**, but if you are willing to hack around with it, you might find it useful.

For this reason there are no binaries yet.

## Working features

These features are fully working:

* Enabling DNSCrypt
* Logging DNS requests

## Partially working features

The following features work but have no UI yet:

* Creation of static routes based on ASN, ASN Organization, Netmask, array of netmasks.

## Planned features

The following features are planned:

* Creation of static routes based on DNS logs
* Manipulation of DNS to fix/specify IP addresses for specified domains
* Creation of OpenDNS VPN connections
* Quick VPN traffic toggling
* 'Easy mode' that sets up features automatically, with profiles for specific websites or apps.

## Supported devices

* Tested - Ubiquiti EdgeRouter based devices (such as EdgeRouter X)
* Untested - Other Vyatta OS based devices.

NOTE: This is unofficial software and is in no way supported or endorsed by Ubiquiti. 

## Support, License, Terms

Using this software may void your warranty and I cannot be held responsible for any damage caused to you, your devices
or your business. You use this software at your own risk and it comes with no warranty or expectation of functionality.
