# Mock Referrals API

This repository contains a mock API that implements the creation, retrieval, and redemption of referrals. This document details setup, operation, design intent, and specifications for the project.

## Local Setup

All dependencies have been installed via NuGet using standard methods. It was developed against dotnet core version `9.0.202` for Apple Silicon. You can download the needed packages via the standard:

```bash
dotnet restore
```

Once they are installed, tests can be run via:

```bash
dotnet test
```

The API itself can be run from the root directory containing the solution file via:

```bash
dotnet run --project MockReferralsApi/MockReferralsApi.csproj
```

An OpenAPI specification JSON file is generated automatically from controller annotations at build time. This can be found in the normal build output directory (`obj/` by default).

## Solutions to Problems From Prompt

> How will existing users create new referrals using their existing referral code?

An HTTP PUT action is provided on the `/referrals` path. It accepts the same DTO that is returned by the index action under the `Referrals` list. There is not currently a path for creating new codes, as that would be a separate resource or process.

> How will the app generate referral links for the Share feature?

Referral links are automatically generated using the referral code associated with a user whenever referrals are requested. This link is not currently saved, in the event the url needs to change. This avoids the need to migrate stored records when a simple routing change is made. Older redemption endpoints can be deprecated and made to redirect to the new version where appropriate.

> How will existing users check the status of their referrals?

The index action will provide a full listing of all created referrals. This potentially introduces a performance issue in the event a dedicate user refers a very large number of people, but that threshold would be quite high and didn't seem appropriate to handle in a simple MVP.

> How will the app know where to direct new users after they install the app via a referral?

Referral redemption requires a user id and a code to be completed. This can be done with a single request, and the API does not currently need to provide the client with any additional context. Upon completion of registration via referral deeplink, the client can send a second request to redeem the referral, or this can be done by the registration process via internal backend requests. (This could also be done by having the registration process invoke the relevant service itself, if it is desirable to avoid having backend services call other backend endpoints themselves.)

> Since users may eventually earn rewards for referrals, should we take extra steps to mitigate abuse?

Without knowing more specifics of the process and business concerns around this, I have implemented very simple uniqueness validation for the phone, email, and redeeming user id associated with a referral. This would not be a robust solution for a production system handling redemptions, but it at least establishes a seam for validating against simple, duplicate redemption.

## Rationale

With any software project, the expression of business logic will require some assumptions to be held and choices to be made.

* The codebase uses the `var` keyword throughout to match current C# style. Additionally, this requires code to be clearly expressed to avoid problems of type ambiguity when reviewing the content. Code exists to explain logic to humans, and it should be written with that goal in mind.


* Though the prompt called out redemption as an unnecessary part of this submission, a simple path has been included for it. While it exists as a separate method on the referrals resource controller, the actual logic of the change is held in the datastore. If a separate request is undesirable, the datastore call can be folded into existing user registration logic easily.


* Referral records include fields for capturing identifying information about the referee. This can be used to guard against the most simple abuses while also helping to capture user data.


* The database backing EntityFramework models is in-memory. No attempt has been made to formally set primary keys, index for performance, or any other common schema setup. It is not necessary for this MVP, and if this code were to be altered to use a traditional database, fewer details would need to be changed to support this. 


* A datastore abstraction and class have been provided to isolate business logic from data operations. This allows easier migration of database backends as well as providing a formal, private API for data operations in the language of the business logic. Additionally, this avoids problems of needing to mock database interactions in unit tests. EntityFramework context and set classes are concrete and do not provide single interfaces to easily mock. Directly calling database operations from a controller is always a complication that should be avoided in controller logic.


* The fake referral link defined in constants does not use the url from the mockups to make it slightly harder for future challenge respondents to find this repo via public internet searches.


* User IDs are currently passed via request header to align with the typical approach of an auth token in the header being used to fetch a current user from an auth guard in the controller.
