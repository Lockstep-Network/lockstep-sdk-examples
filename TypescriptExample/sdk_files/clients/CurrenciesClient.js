/**
 * Lockstep Software Development Kit for JavaScript / TypeScript
 *
 * (c) 2021-2022 Lockstep, Inc.
 *
 * For the full copyright and license information, please view the LICENSE
 * file that was distributed with this source code.
 *
 * @author     Ted Spence <tspence@lockstep.io>
 * @copyright  2021-2021 Lockstep, Inc.
 * @version    2021.39
 * @link       https://github.com/tspence/lockstep-sdk-typescript
 */
export class CurrenciesClient {
    /**
     * Internal constructor for this client library
     */
    constructor(client) {
        this.client = client;
    }
    /**
     * Retrieve a currency conversation rate from one currency to another as of the specified date.              Optionally, you can specify which currency data provider to use.
     *
     *              The currency rate model contains all of the information used to make the API call, plus the rate to              use for the conversion.
     *
     * @param sourceCurrency - The ISO 4217 currency code of the origin currency. For a list of currency codes, call List Currencies.
     * @param destinationCurrency - The ISO 4217 currency code of the target currency. For a list of currency codes, call List Currencies.
     * @param date - The date for which we should cto use for this currency conversion.
     * @param dataProvider - Optionally, you can specify a data provider.
     */
    retrievecurrencyrate(sourceCurrency, destinationCurrency, date, dataProvider) {
        const url = `/api/v1/Currencies/${sourceCurrency}/${destinationCurrency}`;
        const options = {
            params: {
                date,
                dataProvider,
            },
        };
        return this.client.request('get', url, options, null);
    }
    /**
     * Receives an array of dates and currencies and a destination currency and returns an array of the corresponding currency rates to the given destination currency (Limit X).
     *
     * @param destinationCurrency - The currency to convert to.
     * @param body - A list of dates and source currencies.
     */
    bulkcurrencydata(destinationCurrency, body) {
        const url = `/api/v1/Currencies/bulk`;
        const options = {
            params: {
                destinationCurrency,
            },
        };
        return this.client.request('post', url, options, body);
    }
}
//# sourceMappingURL=CurrenciesClient.js.map