SimpleMessage
=======

Orleans-based "SMS" service application for TechEd Australia!

You can run it in 3 ways:

1. In Azure:
* Right click the SimpleMessage Azure project and publish it, then navigate to the cloud service's URL in your browser.
2. Locally using the Azure emulator:
* Set the SimpleMessage Azure project as your default and run it.
* The browser should open to the main page.
3. Locally using the LocalSilo project:
* Note: This was only included for the purpose of testing GrainObservers.
* Run LocalSilo.exe in a cmd window and wait for it to warm up (it will tell you that you can press the any key to exit.)
* Run SmsConsoleClient.exe in one or more cmd windows to see usage.

@reubenbond
