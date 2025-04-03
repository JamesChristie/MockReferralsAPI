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

> How are referrals created?

An HTTP PUT action is provided on the `/referrals` path. It accepts the same DTO that is returned by the index action under the `Referrals` list. There is not currently a path for creating new codes, as that would be a separate resource or process.

> How are referral links generated?

Referral links are automatically generated using the referral code associated with a user whenever referrals are requested. This link is not currently saved, in the event the url needs to change. This avoids the need to migrate stored records when a simple routing change is made. Older redemption endpoints can be deprecated and made to redirect to the new version where appropriate.

> How do users check the current status of created referrals?

The index action will provide a full listing of all created referrals. This potentially introduces a performance issue in the event a dedicate user refers a very large number of people, but that threshold would be quite high and didn't seem appropriate to handle in a simple MVP.

> How does the client handle directing new users during referral redemption?

The redemption controller implements an endpoint to redirect to the app store. In the current form, this is a simple redirect. In the final version of this API, this would include a small HTML document with associated JavaScript to ensure that the deferred deep link is placed into the clipboard for later extraction by the app, once installed. 

The redirection endpoint only ever replies with a 302, regardless of whether the parameters provided are valid. Tihs was done as directing new users to the app is likely preferable to presenting them with an error before they've even entered the flow.

> What steps have been taken to prevent abuse?

Referrals are redeemed with a combination of their id and the referral code. This currently prohibits redemption via code only, which may be undesirable. As a deferred deep link requires placing load bearing data into the clipboard, this process could be interrupted after the initial opening of the referral link. If this occurs, the user can simply re-open the original link after the app is installed.

The combination of id and code is used to ensure that the correct, specific redemption record is updated. Redemption via code only could result in duplicate referrals from the perspective of the referring user. If this is an acceptable outcome, then referral via code only can be added as a case to the redemption controller, and the id param made optional. A new referral record would then be created and redeemed.

Without knowing more specifics of the process and business concerns around this, I have implemented very simple uniqueness validation for the phone, email, and redeeming user id associated with a referral. This would not be a robust solution for a production system handling redemptions, but it at least establishes a seam for validating against simple, duplicate redemption.

## Rationale

With any software project, the expression of business logic will require some assumptions to be held and choices to be made.

* The codebase uses the `var` keyword throughout to match current C# style. Additionally, this requires code to be clearly expressed to avoid problems of type ambiguity when reviewing the content. Code exists to explain logic to humans, and it should be written with that goal in mind.


* Though the prompt called out redemption as an unnecessary part of this submission, a simple path has been included for it. While it exists as a separate method on the referrals resource controller, the actual logic of the change is held in the datastore. If a separate request is undesirable, the datastore call can be folded into existing user registration logic easily.


* Referral records include fields for capturing identifying information about the referee. This can be used to guard against the most simple abuses while also helping to capture user data.


* The database backing EntityFramework models is in-memory. No attempt has been made to formally set primary keys, index for performance, or any other common schema setup. It is not necessary for this MVP, and if this code were to be altered to use a traditional database, fewer details would need to be changed to support this. 


* A datastore abstraction and class have been provided to isolate business logic from data operations. This allows easier migration of database backends as well as providing a formal, private API for data operations in the language of the business logic. Additionally, this avoids problems of needing to mock database interactions in unit tests. EntityFramework context and set classes are concrete and do not provide single interfaces to easily mock. Directly calling database operations from a controller is typically a complication that should be avoided.


* The fake referral link defined in constants does not use the url from the mockups to make it slightly harder for future challenge respondents to find this repo via public internet searches. (The same goes for prompt question text in the above section.)


* User IDs are currently passed via request header to align with the typical approach of an auth token in the header being used to fetch a current user from an auth guard in the controller.
