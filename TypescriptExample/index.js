import { LockstepApi } from "./sdk_files/APIClient.js";

function main()
{
    console.log('Hello, world');   
    var apiKey = process.env["LOCKSTEPAPI_SBX"];
    console.log(apiKey);   
    var client = LockstepApi.withEnvironment('sbx').withApiKey(apiKey);
    console.log(`Found version ${client.version}`);
    client.Status.ping().then(result => {
        console.log('Result: ' + JSON.stringify(result));
    })
}

main();