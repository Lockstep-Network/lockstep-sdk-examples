package io.lockstep;

import java.util.Map;

/**
 * Simple example of Yoctopuce library usage
 *
 */
public class App
{
    public static void main( String[] args )
    {
        Map<String, String> env = System.getenv();
        String key = env.getOrDefault("LOCKSTEPAPI", null);
        LockstepApi client = LockstepApi.withEnvironment("sbx").withApiKey(key);
        LockstepResponse<StatusModel> response = client.getStatusClient().Ping();
        System.out.println(response.isSuccess());
        System.out.println("Hello, World!");
    }
}
