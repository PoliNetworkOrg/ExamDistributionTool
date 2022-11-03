# ExamDistributionTool

## Requirements
.Net Core 6 or upper is required. https://dotnet.microsoft.com/en-us/download/dotnet/6.0

## Description

This tool has two modes of operation: command line and GUI.

Both modes can be either given a properly formatted file or a file containing text copied straight from the exams page of personal services.
The format of both can be found at the bottom of this README. If you give it the copied text, it will ask you the CFU values for each exam.

## GUI
Simply click on the "Load exams from file" button, and select the file.
The possible solutions will be output in order from most desirable to least (so the TOP one is the "best" one).

## Command line tool
Either pass the filepath as argument, or input it directly.
The possible solutions will be output in order from least desireable to most (so the BOTTOM one is the "best" one).

## Input file format

Formatted example:

```
[
  {
    "name": "Nome1",
    "date" : ["2021-01-01", "2021-12-31"],
    "cfu" : "10"
  },
  {
    "name": "Nome2",
    "date" : ["2021-04-01", "2021-05-31"],
    "cfu" : "12"
  },
  {
    "name": "Nome3",
    "date" : ["2021-07-01", "2021-11-30"],
    "cfu" : "5"
  }
]
```

Copied example (this is not a formatting guide, this is merely what it looks like when text is copied from the exam page):
```
Nome1 - 000001  1 Professor1
01/01/2021: Exam
31/12/2021: Exam

  Generic text (such as the missing questionnaire answers warning).
  
Nome2 - 000002   2 Professor2
01/04/2021: Exam
31/05/2021: Exam
 
  Generic text (such as the missing questionnaire answers warning).
   
Nome3 - 000003  1 Professor3
01/07/2021: Exam
30/11/2021: Exam
```
