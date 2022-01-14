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
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
import * as axios from "axios";
import { ActivitiesClient } from "./clients/ActivitiesClient.js";
import { ApiKeysClient } from "./clients/ApiKeysClient.js";
import { AppEnrollmentsClient } from "./clients/AppEnrollmentsClient.js";
import { ApplicationsClient } from "./clients/ApplicationsClient.js";
import { AttachmentsClient } from "./clients/AttachmentsClient.js";
import { CodeDefinitionsClient } from "./clients/CodeDefinitionsClient.js";
import { CompaniesClient } from "./clients/CompaniesClient.js";
import { ContactsClient } from "./clients/ContactsClient.js";
import { CreditMemoAppliedClient } from "./clients/CreditMemoAppliedClient.js";
import { CurrenciesClient } from "./clients/CurrenciesClient.js";
import { CustomFieldDefinitionsClient } from "./clients/CustomFieldDefinitionsClient.js";
import { CustomFieldValuesClient } from "./clients/CustomFieldValuesClient.js";
import { DefinitionsClient } from "./clients/DefinitionsClient.js";
import { EmailsClient } from "./clients/EmailsClient.js";
import { InvoiceHistoryClient } from "./clients/InvoiceHistoryClient.js";
import { InvoicesClient } from "./clients/InvoicesClient.js";
import { LeadsClient } from "./clients/LeadsClient.js";
import { MigrationClient } from "./clients/MigrationClient.js";
import { NotesClient } from "./clients/NotesClient.js";
import { PaymentApplicationsClient } from "./clients/PaymentApplicationsClient.js";
import { PaymentsClient } from "./clients/PaymentsClient.js";
import { ProvisioningClient } from "./clients/ProvisioningClient.js";
import { ReportsClient } from "./clients/ReportsClient.js";
import { StatusClient } from "./clients/StatusClient.js";
import { SyncClient } from "./clients/SyncClient.js";
import { UserAccountsClient } from "./clients/UserAccountsClient.js";
import { UserRolesClient } from "./clients/UserRolesClient.js";
export class LockstepApi {
    /**
     * Internal constructor for the Lockstep API client
     */
    constructor(customUrl) {
        this.version = "2021.39.690";
        this.bearerToken = null;
        this.apiKey = null;
        this.serverUrl = customUrl;
        this.Activities = new ActivitiesClient(this);
        this.ApiKeys = new ApiKeysClient(this);
        this.AppEnrollments = new AppEnrollmentsClient(this);
        this.Applications = new ApplicationsClient(this);
        this.Attachments = new AttachmentsClient(this);
        this.CodeDefinitions = new CodeDefinitionsClient(this);
        this.Companies = new CompaniesClient(this);
        this.Contacts = new ContactsClient(this);
        this.CreditMemoApplied = new CreditMemoAppliedClient(this);
        this.Currencies = new CurrenciesClient(this);
        this.CustomFieldDefinitions = new CustomFieldDefinitionsClient(this);
        this.CustomFieldValues = new CustomFieldValuesClient(this);
        this.Definitions = new DefinitionsClient(this);
        this.Emails = new EmailsClient(this);
        this.InvoiceHistory = new InvoiceHistoryClient(this);
        this.Invoices = new InvoicesClient(this);
        this.Leads = new LeadsClient(this);
        this.Migration = new MigrationClient(this);
        this.Notes = new NotesClient(this);
        this.PaymentApplications = new PaymentApplicationsClient(this);
        this.Payments = new PaymentsClient(this);
        this.Provisioning = new ProvisioningClient(this);
        this.Reports = new ReportsClient(this);
        this.Status = new StatusClient(this);
        this.Sync = new SyncClient(this);
        this.UserAccounts = new UserAccountsClient(this);
        this.UserRoles = new UserRolesClient(this);
    }
    /**
     * Construct a new Lockstep API client to target the specific environment.
     *
     * @param env The environment to use, either "prd" for production or "sbx" for sandbox.
     * @returns The Lockstep API client to use
     */
    static withEnvironment(env) {
        var url = "https://api.lockstep.io";
        switch (env) {
            case "prd":
                url = "https://api.lockstep.io";
                break;
            case "sbx":
                url = "https://api.sbx.lockstep.io";
                break;
        }
        return new LockstepApi(url);
    }
    /**
     * Construct an unsafe client that uses a non-standard server; this can be necessary
     * when using proxy servers or an API gateway.  Please be careful when using this
     * mode.  You should prefer to use `withEnvironment()` instead wherever possible.
     *
     * @param unsafeUrl The non-Lockstep URL to use for this client
     * @returns The Lockstep API client to use
     */
    static withCustomEnvironment(unsafeUrl) {
        return new LockstepApi(unsafeUrl);
    }
    /**
     * Configure this Lockstep API client to use a JWT bearer token.
     * More documentation is available on [JWT Bearer Tokens](https://developer.lockstep.io/docs/jwt-bearer-tokens).
     *
     * @param token The JWT bearer token to use for this API session
     */
    withBearerToken(token) {
        this.bearerToken = token;
        this.apiKey = null;
        return this;
    }
    /**
     * Configures this Lockstep API client to use an API Key.
     * More documentation is available on [API Keys](https://developer.lockstep.io/docs/api-keys).
     *
     * @param apiKey The API key to use for this API session
     */
    withApiKey(apiKey) {
        this.apiKey = apiKey;
        this.bearerToken = null;
        return this;
    }
    /**
     * Construct headers for a request
     */
    getHeaders() {
        if (this.apiKey !== null) {
            return {
                'Api-Key': this.apiKey,
            };
        }
        if (this.bearerToken !== null) {
            return {
                'Authorization': `Bearer ${this.bearerToken}`,
            };
        }
        return {};
    }
    /**
     * Make a GET request using this client
     */
    request(method, path, options, body) {
        return __awaiter(this, void 0, void 0, function* () {
            const requestConfig = {
                url: new URL(path, this.serverUrl).href,
                method,
                params: options,
                data: body,
                headers: this.getHeaders(),
            };
            var result = yield axios.default.request(requestConfig);
            if (result.status >= 200 && result.status < 300) {
                return result.data;
            }
            else {
                return result.data;
            }
        });
    }
}
//# sourceMappingURL=APIClient.js.map