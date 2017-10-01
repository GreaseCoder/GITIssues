# GITIssues
Your task is to integrate with the Github REST API to search for issues in the Angular Github repo for the
previous 7 days.

## Running the Application
Executables have been built for win7-x64 and win10-x64.  They can be found here:
```
TakeNote/TakeNote/bin/Release/netcoreapp1.1/win7-x64/public/GITIssues.exe
TakeNote/TakeNote/bin/Release/netcoreapp1.1/win10-x64/public/GITIssues.exe
```
Running the executable will launch the application, and provide you a default port to access the API through.

## Additional
I also included an alternative implementation of IssueCollection (Models/Issue_Alternative.cs) which uses Octokit instead of parsing the API manually.