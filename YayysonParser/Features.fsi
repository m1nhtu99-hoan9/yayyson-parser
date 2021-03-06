[<AutoOpen>]
module YayysonParser.Features

open System


val ParseToGuid: string -> Result<Guid, string>
val ParseToTimeSpan: string -> Result<TimeSpan, string>

val ParseAndCastToDateTime: string -> Result<DateTime, string>
val ParseAndCastToGuid: string -> Result<Guid, string>
val ParseAndCastToTimeSpan: string -> Result<TimeSpan, string>
val ParseAndCastToFloat: string -> Result<float32, string>
val ParseAndCastToInt32: string -> Result<int, string>
