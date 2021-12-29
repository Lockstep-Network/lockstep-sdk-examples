import { LockstepApi } from 'lockstep-sdk';

export function main()
{
    var client = LockstepApi.withEnvironment('prd');
    client.Status.ping().then(result => {
        console.log('Result: ' + JSON.stringify(result));
    })
}