namespace mor.FreeDB
{
    public enum ResponseCode
    {
        CODE_200, // Exact match 
        CODE_202, // No match 
        CODE_210, // Okay // or in a query multiple exact matches
        CODE_211, // InExact matches found - list follows

        CODE_401, // sites: no site information available
        CODE_402, // Server Error
        CODE_403, // Database entry is corrupt
        CODE_409, // No Handshake

        CODE_500, // Invalid command, invalid parameters, etc.
    }
}
