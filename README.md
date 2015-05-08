# Build-2015-Drummer
Low-latency audio drum sample from Build 2015 Day 2 keynote

Example from John Shewchuk's demo in the Day 2 keynote for Build 2015. This shows how to use the new low-latency-optimized AudioGraph API to load drum samples and play them back through a C#/XAML app.


Note: Not all devices currently have good audio latency characteristics. Surface Pro 3 is quite good as are most other PCs using the same chipset. Your mileage will vary based upon the build of Windows and the availability of updated audio drivers.

For more details on AudioGraph, see:
Build 2015 session: http://channel9.msdn.com/Events/Build/2015/3-634
Official AudioGraph sample: https://github.com/Microsoft/Windows-universal-samples/tree/master/audiocreation
