Partial Key Verification Library
================================
Partial Key Verification (PKV) is a software licensing key technique that breaks 
a product key into multiple subkeys and with each version of your software you 
check a different subset of the subkeys.  The beauty of this approach is that a 
cracker cannot generate a complete keygen.  They might be able to generate one 
for a given version, but it won't work on different release (assuming you check
a different subkey).  Another nice feature of PVK is that the key contains an 
embedded serial number.  This allows you to easily check the key against a list 
of stolen/posted/refunded keys. For more information about the PKV technique, 
see [this blog post by Brandon Staggs](http://www.brandonstaggs.com/2007/07/26/implementing-a-partial-serial-number-verification-system-in-delphi/).

My version of PKV differs slightly from the one discussed by Brandon Staggs.  
Instead of using 8-bit subkeys, I used 32-bit subkeys (just check one key instead 
of four). My version also Base-32 (5-bit) encodes the keys to shrink the key size 
by 20%, and allows you to specify a different hash algorithm for each subkey.

This is a .NET 4.0 portable library and distributed under the Simplified BSD license.

NuGet
-----
PM> Install-Package Partial.Key.Verification

Example
-------
To generate a key, create a PartialKeyGenerator class specifying the checksum 
and hash functions to use, along with the base values for each subkey. Then 
call the Generate function, passing it a serial number or a string (such as 
the customer's e-mail address) to generate a key. You can optionally tell the 
generator to add a separator between a certain number of characters in the 
key by setting the Spacing property.

    var generator = new PartialKeyGenerator(new Adler16(), new Jenkins96(), new uint[] { 1, 2, 3, 4 }) { Spacing = 6 };
    var key = generator.Generate("bob@smith.com");

This will generate the key: QDKZUO-JLLWPY-XWOULC-ONCQIN-5R5X35-ZS3KEQ. Adler16 is 
the checksum function and Jenkins96 is the hash function. You can have as many 
subkeys as you like, but each subkey adds 7 more characters to the key.

To validate the key, use the PartialKeyValidator static class. Again telling it 
the checksum and hash functions to use, along with which subkey to check and the 
base value for that subkey. For example, to check the first subkey of the key 
generated above:

    var isValid = PartialKeyValidator.ValidateKey(new Adler16(), new Jenkins96(), key, 0, 1);
	
See the unit tests for more examples.
