# Yayyson Parser

```
[GUID]  
"${Guid.NewGuid}" -> bb768ef2-cfdd-4980-a0a7-3c99b301f988
"${Guid.Parse 60D703A0-839B-48DF-9EC3-8AA39F41840E}" -> 60d703a0-839b-48df-9ec3-8aa39f41840e
"${Guid.Empty}" -> 00000000-0000-0000-0000-000000000000

[TimeSpan]  
"${17.413906d_2.8342848h_60.51424m15.415337s}" -> 17.13:46:51.1734170
"${97h + 54d_57h}" -> 60.10:00:00
"${23.644646d - 62.555393d_6.375373h_7.083997m17.367828s}" -> -39.04:21:22.2912480
"${323.3027 + 60.641285d_4.805139h_22.670769m_23.144062s}" -> 
	Failed: Unsupported operation: Addition of FloatLiteral to TimeSpanLiteral.

[DateTime]  
"${DateTime.MinValue}" -> 1/01/0001 12:00:00 AM
"${DateTime.MaxValue}" -> 31/12/9999 11:59:59 PM
"${DateTime.Now}" -> 30/12/2021 1:01:39 PM
"${DateTime.UtcNow}" -> 30/12/2021 6:01:39 AM
"${DateTime.MinValue + 77.633896d}" -> 19/03/0001 3:12:48 PM
"${DateTime.Now - 69.94485d_21.576208h}" -> 20/10/2021 4:46:30 PM
"${DateTime.UtcNow + 69.25352d_21.070854h_19.891806m_52.440514s}" -> 10/03/2022 9:31:44 AM
"${DateTime.Now + 8.419537}" -> 
	Failed: Unsupported operation: Addition of DateTimeLiteral to FloatLiteral.
"${8.239782d7.77775h + DateTime.UtcNow}" -> 
	Failed: Invalid operation: Addition of TimeSpan to DateTime. Try swapping the operands?

[Float32]  
"${707.12164}" -> 707.1216431f
"${687.3963 + 940}" -> 1627.39624f
"${167 - 412.99237}" -> -245.9923706f
"${548.6907 + 448.88324}" -> 997.5739136f
"${12.164541 - 596.1765}" -> -584.0119629f

[Int32]  
"${251}" -> 251
"${259 + 545}" -> 804
"${143 - 360}" -> -217
```

## Reference

To use this library in C#:

```csharp
using static YayysonParser.Features;
```

For more code examples, please refer to the unit tests written in C#.

### `ParseToGuid`

```csharp
Microsoft.FSharp.Core.FSharpResult<Guid, string> ParseToGuid(string str)
```

Examples:

```csharp
var guidResult0 = ParseToGuid("${Guid.NewGuid}");
Console.WriteLine(guidResult0.IsOk);	            // True
Console.WriteLine(guidResult0.IsError);	            // False
Console.WriteLine(guidResult0.ResultValue);	    // 52a4878a-20e2-493b-b1af-cb2e2834779f
Console.WriteLine(null == guidResult0.ErrorValue);  // True
```

```csharp
var guidResult1 = ParseToGuid("${Guid.Parse not-even-a-valid-guid}");
Console.WriteLine(guidResult1.IsOk);         // False
Console.WriteLine(guidResult1.IsError);	     // True
Console.WriteLine(guidResult1.ResultValue);  // 00000000-0000-0000-0000-000000000000
Console.WriteLine(guidResult1.ErrorValue);
/*
Error in Yayyson Guid expression: Ln: 1 Col: 14
${Guid.Parse not-even-a-valid-guid}
             ^
Expecting: hexadecimal digit or '-'
Other error messages:
  "Unrecognized Guid format."
*/  		 
```

### `ParseToTimeSpan`

```csharp
FSharpResult<TimeSpan, string> ParseToTimeSpan(string str)
```

Examples:

```csharp
// `_` seperator is optional
var timeSpanResult0 = ParseToTimeSpan("${20.25d_62.125h_83m99.05s}");
Console.WriteLine(timeSpanResult0.IsOk);	        // True
Console.WriteLine(timeSpanResult0.IsError);		// False
Console.WriteLine(timeSpanResult0.ResultValue);		// 22.21:32:09.0500000
Console.WriteLine(null == timeSpanResult0.ErrorValue);  // True
```

```csharp
var timeSpanResult1 = ParseToTimeSpan("${89,9095d}");
Console.WriteLine(timeSpanResult1.IsOk);		// False
Console.WriteLine(timeSpanResult1.IsError);		// True
Console.WriteLine(timeSpanResult1.ResultValue);		// 00:00:00
Console.WriteLine(timeSpanResult1.ErrorValue);  
/*
Error in Yayyson TimeSpan expression: Ln: 1 Col: 8
${0.5h0d7m9s}
       ^
Expecting: 'm' or 's'
*/
```

### `ParseAndCastToDateTime`

```csharp
FSharpResult<DateTime, string> ParseAndCastToDateTime(string str)
```

Examples:

```csharp
var dateTimeResult0 = ParseAndCastToDateTime("${DateTime.MinValue + 89.9095d2h}");
Console.WriteLine(dateTimeResult0.IsOk);		// True
Console.WriteLine(dateTimeResult0.IsError);		// False
Console.WriteLine(dateTimeResult0.ResultValue);		// 31/03/0001 11:49:40 PM
Console.WriteLine(null == dateTimeResult0.ErrorValue);  // True
```

```csharp
var dateTimeResult1 = ParseAndCastToDateTime("${DateTime.Now + 2.022}");
Console.WriteLine(dateTimeResult1.IsOk);		// False
Console.WriteLine(dateTimeResult1.IsError);		// True
Console.WriteLine(dateTimeResult1.ResultValue);		// 1/01/0001 12:00:00 AM
Console.WriteLine(dateTimeResult1.ErrorValue);  
/*
Unsupported operation: Addition of DateTimeLiteral to FloatLiteral.
*/
```

### `ParseAndCastToTimeSpan`

```csharp
FSharpResult<TimeSpan, string> ParseAndCastToTimeSpan(string str)
```

Examples:

```csharp
var timeSpanResult2 = ParseAndCastToTimeSpan("${11d2m9s + 89.9095d}");
Console.WriteLine(timeSpanResult2.IsOk);		// True
Console.WriteLine(timeSpanResult2.IsError);		// False
Console.WriteLine(timeSpanResult2.ResultValue);		// 100.21:51:49.8000000
Console.WriteLine(null == timeSpanResult2.ErrorValue);  // True
```

```csharp
var timeSpanResult3 = ParseAndCastToTimeSpan("${DateTime.MinValue + 3h33s}");
Console.WriteLine(timeSpanResult3.IsOk);		// False
Console.WriteLine(timeSpanResult3.IsError);		// True
Console.WriteLine(timeSpanResult3.ResultValue);		// 00:00:00
Console.WriteLine(timeSpanResult3.ErrorValue);  
/*
Invalid operation: Addition of TimeSpan to DateTime. Try swapping the operands?
*/
```

### `ParseAndCastToFloat`

```csharp
FSharpResult<float, string> ParseAndCastToFloat(string str)
```

Examples:

```csharp
var floatResult0 = ParseAndCastToFloat("${2299 - 2088.88}");
Console.WriteLine(floatResult0.IsOk);	             // True
Console.WriteLine(floatResult0.IsError);             // False
Console.WriteLine(floatResult0.ResultValue);	     // 210.12012
Console.WriteLine(null == floatResult0.ErrorValue);  // True
```

```csharp
var floatResult1 = ParseAndCastToFloat("${3489 - 3298 + 1986}");
Console.WriteLine(floatResult1.IsOk);		      // False
Console.WriteLine(floatResult1.IsError);	      // True
Console.WriteLine(floatResult1.ResultValue);	      // 0
Console.WriteLine(floatResult1.ErrorValue);  
/*
Given expression not evaluated to System.Single, but System.Int32
*/
```

### `ParseAndCastToInt32`

```csharp
FSharpResult<int, string> ParseAndCastToInt32(string str)
```

Examples:

```csharp
var intResult0 = ParseAndCastToInt32("${3489 - 3298 + 1986}");
Console.WriteLine(intResult0.IsOk);		   // True
Console.WriteLine(intResult0.IsError);		   // False
Console.WriteLine(intResult0.ResultValue);	   // 2177
Console.WriteLine(null == intResult0.ErrorValue);  // True
```

```csharp
var floatResult1 = ParseAndCastToInt32("${221.75 + 38}");
Console.WriteLine(floatResult1.IsOk);		   // False
Console.WriteLine(floatResult1.IsError);	   // True
Console.WriteLine(floatResult1.ResultValue);	   // 0
Console.WriteLine(floatResult1.ErrorValue);  
/*
Given expression not evaluated to System.Int32, but System.Single
*/
```

## Licence

MIT licensed. (c) 2021 MinhTu Thomas Hoang 
