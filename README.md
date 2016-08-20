# Azure Reactive Chirper Sample #

An Azure-hosted version of a web-base Chirper-like sample, using the Reactive Computations.

Runs an Orleans silo in an Azure worker role, and an Azure web role acting as a client talking to the grains in the silo.


## Structure ##



The sample is configured to run inside of the Azure Compute Emulator on your desktop by default.

More info about the hello world azure sample, from which this sample is derived is available here:
http://dotnet.github.io/orleans/Samples-Overview/Azure-Web-Sample
