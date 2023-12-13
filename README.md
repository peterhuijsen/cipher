# Cipher
After initialization and cloning of the project you can do the following two things:
1. Run a console application which will encrypt and decrypt data by the given key in the console. For this you will need to configure the input text file from which the program will read the text which should be transformed. This can be done in the `appsettings.json` file where you can change this path and the type of transformation which should be applied (encryption, decryption; vigenere, caesar, etc.).
2. Run two instances of the client application via the console by running the command `dotnet run --urls=http://localhost:5000` and `dotnet run --urls=http://localhost:5001`. Here, in the `appsettings.json` file you can change the alphabet which should be used for encryption and decryption between the client. Now, you can open the postman project (https://www.postman.com/orange-meadow-579510/workspace/cipher-workspace/overview) and run the initialization request to initialize a new session. Then you can send message between the clients with the other requests. Variables can be edited in the collection.

The main code can be found in the cipher project, which is basically a package wich contains useful services which are used in the actual production projects.
It following settings are available for the cipher package:
```yaml
Cipher:
  Mode: Decypt | Encrypt # How the program should shift the input characters, back or forwards, decrypt or encrypt.
  Type: Vigenere | Caesar # The type of cipher the program should use.
  Encoding: Text | Base64 # The type of encoding of the input file, how it should decode the input to data.

  Alphabet: string # The alphabet which the program should use for its cipher.
  Input: string # The path to the input file for the console application
```
Most of these settings will not have an impact on the functionality of the API clients.
