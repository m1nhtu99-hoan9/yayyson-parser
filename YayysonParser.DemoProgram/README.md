# Yayyson Parser: Demo Application

```
YAYYSON PARSER DEMO

\(^_^ ) GUID \(^_^ )

"${Guid.NewGuid}" -> bb768ef2-cfdd-4980-a0a7-3c99b301f988

"${Guid.Parse 60D703A0-839B-48DF-9EC3-8AA39F41840E}" -> 60d703a0-839b-48df-9ec3-8aa39f41840e

"${Guid.Empty}" -> 00000000-0000-0000-0000-000000000000



\(^_^ ) TimeSpan \(^_^ )

"${17.413906d_2.8342848h_60.51424m15.415337s}" -> 17.13:46:51.1734170

"${97h + 54d_57h}" -> 60.10:00:00

"${23.644646d - 62.555393d_6.375373h_7.083997m17.367828s}" -> -39.04:21:22.2912480

"${323.3027 + 60.641285d_4.805139h_22.670769m_23.144062s}" -> 
	Failed: Unsupported operation: Addition of FloatLiteral to TimeSpanLiteral.



\(^_^ ) DateTime \(^_^ )

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



\(^_^ ) Float32 \(^_^ )

"${707.12164}" -> 707.1216431f

"${687.3963 + 940}" -> 1627.39624f

"${167 - 412.99237}" -> -245.9923706f

"${548.6907 + 448.88324}" -> 997.5739136f

"${12.164541 - 596.1765}" -> -584.0119629f



\(^_^ ) Int32 \(^_^ )

"${251}" -> 251

"${259 + 545}" -> 804

"${143 - 360}" -> -217

```