# The Tide Developer challenge - 2023
The [Dev2023 challenge](http://h4DevChallenge.tide.org) is a technical puzzle challenge using  Tide Protocol's novel user authentication and digital protection technology, aimed at assessing the thinking, skills and personalities of software developers in areas relevant to Tide.

This challenge is part of an engagement program by the [Tide Foundation](https://tide.org) with a specific focus on Tide's next-generation technology: A new technology that grants access using keys **NO ONE** will ever hold. Not even Tide! 

## Let's go
The concept of this challenge is simple: A secret code is hidden and can only be unlocked when the correct password is entered. Get the code - and you solved the challenge!  The password authentication process is obfuscated and decentralized using Tide's [PRISM](https://github.com/tide-foundation/DevChallenge2023/blob/main/diagrams/svg/DevChallenge2023_prism.svg) cryptography - the world's most secure password authentication[^pwd].  In this challenge, only one ORK[^ork] node performs the authentication.  One ORK's internal data is secret but its processes are available here transparently.  The entire source code for the challenge, together with full documentation, is offered herewith for those wishing to take a deeper look.  The user flow can be found below and the full technical diagram can be found [here](https://github.com/tide-foundation/DevChallenge2023/blob/main/diagrams/svg/DevChallenge2023_SignInFlow.svg).

## User Flow Diagram
![alt text](https://github.com/tide-foundation/DevChallenge2023/blob/main/diagrams/svg/DevChallenge2023_userflow.svg "Flow Diagram")

## Components
1. **DevChallenge-Node** - Minimal version of the Tide ORK, specific to this challenge.
1. **DevChallenge-TinySDK** - Minimal SDK for front-end website integration.
1. **DevChallenge-front** - Front-end website for this challenge.
    1. **Modules/DevChallenge-TideJS** - Tide Libraries including encryption + PRISM
1. **Diagrams** -  Diagrams for this challenge.
    1. [**Technical diagram**](https://raw.githubusercontent.com/tide-foundation/DevChallenge2023/main/diagrams/svg/DevChallenge2023_SignInFlow.svg) - A technical diagram of the challenge.  
    2. [**PRISM**](diagrams/svg/DevChallenge2023_prism.svg) - The mathematical diagram of Tide's PRISM. 
    3. [**User flow**](https://github.com/tide-foundation/DevChallenge2023/blob/main/diagrams/svg/DevChallenge2023_userflow.svg) - A user flow diagram. 

# Installation
This guide aims to assist you in replicating the entire challenge environment locally, with 1 ORK node - so you can run it yourself freely.

While all the components of the environment are cross-platform, this manual describes how to set it up in a Windows environment. Similar steps can be followed to achieve the same on Linux.

There is also a [video](https://vimeo.com/780973408/d5df625214) to help you with the installation steps.

## Prerequisite

The following components are required to be set up ahead of the deployment:
1. [.NET 6 Build apps - SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0 ".net Core 6 Download")
1. Clone Repository (`git clone https://github.com/tide-foundation/DevChallenge2023/`)

## Deployment
### ORKs
Open a CMD terminal (not powershell)
````
cd DevChallenge2023\DevChallenge-Node\DevChallenge-Node
set ISPUBLIC=true
set PRISM_VAL=12345
dotnet run --urls=http://localhost:6001
````

As you would want to generate cryptographically secure PRISM_VAL values, follow the [Debug Web Page](https://github.com/tide-foundation/DevChallenge2023#debug-web-page) steps to host the debug web page and click on the button 'Get Random'.

Much like the ORKs that are running in the cloud, both of your ORKs have:
1. Different visibilities
2. Different PRISM values

To test this, navigate to http://localhost:6001/prizeKey. Notice how a value appears. In contrast, navigating to that same page will show PAGE NOT FOUND if the environment variable ISPUBLIC in the terminal set to false.

### Static Web Page
Go to `DevChallenge2023\DevChallenge-front\js`

In `shifter.js`, modify line 205 so that the front-end page will contact your local ORKs:
From this: `urls: ["https://devchallenge-ork1.azurewebsites.net"],`
To this: `urls: ["http://localhost:6001"],`

Now to host the front-end webpage; this guide will use a simple Python http server, but you can you anything you like.

Host the page with Python:
````
python -m http.server 9000
````

Navigating to http://localhost:9000 will take you with the Tide Dev Challenge welcome page (similar to https://DevChallenge2023.tide.org).

### Debug Web Page 
NOTE: This is only if you'd like to test your local ORKs with encryption/decryption of your own data with your own password

````
cd DevChallenge2023\DevChallenge-front\modules\DevChallenge-TideJS
````

If you look at the file \test\tests.js, you'll see a bunch of functions with different names, e.g. test1, test2...

These are tests we used for debugging purposes. They can also help you understand the different modules of Tide-JS such as AES, NodeClient and PrismFlow (when you just want to encrypt data).

Start a server in the directory where test.html is:
````
python -m http.server 8000
````

Navigate to http://localhost:8000/test.html where you'll see a VERY simple webpage.

Clicking each button will run the corresponding test in test.js. **The output of the function will be in the console.**

## Test
### Encrypting your own data
In the DevChallenge-TideJS directory (DevChallenge2023\DevChallenge-front\modules\DevChallenge-TideJS):
1. In test4 function of test/test.js, change "AAA" to any password of your choosing. Also change "Example" to anything you would like to encrypt.
2. Go to http://localhost:8000/test.html and press F5 (to reload the page)
3. Right-click -> inspect -> console
4. Click the button 'Test 4'
5. Should show a base64 encoded text in console

### Decrypting your own data
In the DevChallenge-front directory:
1. Modify the index.html file:

    Change this line: `<p hidden id="test">G4GmY31zIa35tEwck14URCEAIjeTA8NV+DgjHpngxASGnTU=</p>`
    
    To: `<p hidden id="test">{Your base64 encrypted data from before}</p>`

2. Go to http://localhost:9000) and press F5 (to reload the page)
3. You should see the page with the dots.
4. Enter your password to see if it is able to decrypt!

Question: *So what was the data encrypted with?*

It was encrypted with the hash of a 'key point'[^key] only known to the *user who knows the password + has access to the ORK*.

In essence: ***key point = passwordPoint * PRISM***

Where passwordPoint is a point derived from the user's password. 

Even if someone knows PRISM, they still have to brute-force all password possibilities (which is why this is an online process that is be throttled by the ORK, hence lowering their probably of success to virtually zero).

## Troubleshooting
Ask for any help in the Discord channel! The community and our devs are there for you.

## A Quick Note on the Throttling
You may notice that regardless if you entered the right password or not, the ORKs will throttle you after few attempts. This is due to the fact that it is virtually IMPOSSIBLE (unless you break Eliptic Curve cryptography) for the ORKs to determine what password the user is trying and whether its correct or not (specifically, in this challenge). All the ORKs do is apply their partial PRISM value to a point. Therefore, since the ORKs have no idea what the password is and since the user is obfuscating their password point with a random number, it guarantees that the ORKs 'authenticate' the user without any knowledge of their password. Cool, right?

# More info
[The Tide Website](https://tide.org)

## Get in touch!

[Tide Discord](https://discord.gg/42UCeW4smw)

[Tide Twitter](https://twitter.com/tidefoundation)

  <a href="https://tide.org/licenses_tcoc2-0-0-en">
    <img src="https://img.shields.io/badge/license-TCOC-green.svg" alt="license">
  </a>
</p>

[^pwd]: Tide's focus on developing the world's most secure online password authentication mechanism is because passwords still, unfortunately, are the most common online authentication mechanism used. In general, password authentication is a significantly inferior mechanism compared to its many alternatives. Most of the alternatives (e.g. MFA, passwordless, FIDO2, etc) also suffer from security risks which Tide's authentication helps alleviate. Tide's superior password protection mechanism isn't intended to discourage users from switching to a better alternative, instead offers a better interim-measure until such inevitable switch occurs.
[^ork]: Tide's decentralized network is made of many nodes named ORKs, which stands for Orchestrated Recluder of Keys. A single ORK operates more like a drone in a hive than a node in a network as it performs work that's unique to it and is vastly different than other ORKs. That work is entirely incomprehensive by itself, even to itself. Meaning, the network perform a process where each ORK performs part of that process without knowing or understanding anything about the process itself. Only after the ORKs complete their parts (which is done in parallel), the network produces a meaningful result. This "incomprehensible partial processing", or as we call it "Blind Secret Processing" is done using Tide's groundbreaking new Threshold Cryptography done in Multi-Party Computation.
[^key]: Tide's specific 'key point' is a representation of a cryptographic key as a geometric point on an Edward25519 Elliptic Curve.
