namespace UtilityLayer.Convertors;

public static class URLProcessor
{
    public static string GetMainDomain(string url)
    {
        Uri uri = new Uri(url);
        string host = uri.Host; // Gets the full host (e.g., "www.example.co.uk")

        // Split the host by dots and reverse it (to handle cases like co.uk, com.br, etc.)
        string[] hostParts = host.Split('.').Reverse().ToArray();

        // Check if the host has at least two parts (domain and TLD)
        if (hostParts.Length >= 2)
        {
            return hostParts[1]; // The main domain name is usually one part before the TLD
        }

        return host; // Return the host as is if it's not in a standard format
    }
}