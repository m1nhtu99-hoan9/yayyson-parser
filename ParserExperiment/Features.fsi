module ParserExperiments.Features


open System


val ParseToGuid: string -> Result<Guid, string>

val ParseToTimeSpan: string -> Result<TimeSpan, string>

