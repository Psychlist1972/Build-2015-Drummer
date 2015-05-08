# Build-2015-Drummer
Low-latency audio drum sample from Build 2015 Day 2 keynote

Example from John Shewchuk's demo in the Day 2 keynote for Build 2015. This shows how to use the new low-latency-optimized AudioGraph API to load drum samples and play them back through a C#/XAML app.

Recommendations for your own further investigation:
- Add in effects like echo and reverb, with user-controllable parameters
- Add in the ability to save the performance to a file

Note: At the time of this commit, audio latency characteristics vary considerably between different PCs. Surface Pro 3 is quite good as are most other PCs using the same chipset. Your mileage will vary based upon the build of Windows and the availability of updated low-latency audio drivers.

Note 2: On the Build preview version of Windows, there are some devices where the attack for the sample gets cut off giving the sound a mushy reverb-tail like quality. This is a known bug that will be fixed for Windows 10 RTM.

For more details on AudioGraph, see:

Build 2015 session: http://channel9.msdn.com/Events/Build/2015/3-634

Official AudioGraph sample: https://github.com/Microsoft/Windows-universal-samples/tree/master/audiocreation
