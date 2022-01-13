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
export class CompaniesClient {
    /**
     * Internal constructor for this client library
     */
    constructor(client) {
        this.client = client;
    }
    /**
     * Retrieves the Company specified by this unique identifier, optionally including nested data sets.  A Company represents a customer, a vendor, or a company within the organization of the account holder. Companies can have parents and children, representing an organizational hierarchy of corporate entities. You can use Companies to track projects and financial data under this Company label.
     *
     * See [Vendors, Customers, and Companies](https://developer.lockstep.io/docs/companies-customers-and-vendors) for more information.
     *
     * @param id - The unique Lockstep Platform ID number of this Company; NOT the customer's ERP key
     * @param include - To fetch additional data on this object, specify the list of elements to retrieve.        Available collections: Attachments, Contacts, CustomFields, Invoices, Notes, Classification
     */
    retrieveCompany(id, include) {
        const url = `/api/v1/Companies/${id}`;
        const options = {
            params: {
                include,
            },
        };
        return this.client.request('get', url, options, null);
    }
    /**
     * Updates a Company that matches the specified id with the requested information.
     *
     * The PATCH method allows you to change specific values on the object while leaving other values alone.  As input you should supply a list of field names and new values.  If you do not provide the name of a field, that field will remain unchanged.  This allows you to ensure that you are only updating the specific fields desired.  A Company represents a customer, a vendor, or a company within the organization of the account holder. Companies can have parents and children, representing an organizational hierarchy of corporate entities. You can use Companies to track projects and financial data under this Company label.
     *
     * See [Vendors, Customers, and Companies](https://developer.lockstep.io/docs/companies-customers-and-vendors) for more information.
     *
     * @param id - The unique Lockstep Platform ID number of this Company; NOT the customer's ERP key
     * @param body - A list of changes to apply to this Company
     */
    updateCompany(id, body) {
        const url = `/api/v1/Companies/${id}`;
        return this.client.request('patch', url, null, body);
    }
    /**
     * Disable the Company referred to by this unique identifier.
     *
     * A Company represents a customer, a vendor, or a company within the organization of the account holder. Companies can have parents and children, representing an organizational hierarchy of corporate entities. You can use Companies to track projects and financial data under this Company label.
     *
     * See [Vendors, Customers, and Companies](https://developer.lockstep.io/docs/companies-customers-and-vendors) for more information.
     *
     * @param id - The unique Lockstep Platform ID number of this Company; NOT the customer's ERP key
     */
    disableCompany(id) {
        const url = `/api/v1/Companies/${id}`;
        return this.client.request('delete', url, null, null);
    }
    /**
     * Creates one or more Companies from a given model.  A Company represents a customer, a vendor, or a company within the organization of the account holder. Companies can have parents and children, representing an organizational hierarchy of corporate entities. You can use Companies to track projects and financial data under this Company label.
     *
     * See [Vendors, Customers, and Companies](https://developer.lockstep.io/docs/companies-customers-and-vendors) for more information.
     *
     * @param body - The Companies to create
     */
    createCompanies(body) {
        const url = `/api/v1/Companies`;
        return this.client.request('post', url, null, body);
    }
    /**
     * Queries Companies for this account using the specified filtering, sorting, nested fetch, and pagination rules requested.
     *
     * More information on querying can be found on the [Searchlight Query Language](https://developer.lockstep.io/docs/querying-with-searchlight) page on the Lockstep Developer website.  A Company represents a customer, a vendor, or a company within the organization of the account holder. Companies can have parents and children, representing an organizational hierarchy of corporate entities. You can use Companies to track projects and financial data under this Company label.
     *
     * See [Vendors, Customers, and Companies](https://developer.lockstep.io/docs/companies-customers-and-vendors) for more information.
     *
     * @param filter - The filter for this query. See [Searchlight Query Language](https://developer.lockstep.io/docs/querying-with-searchlight)
     * @param include - To fetch additional data on this object, specify the list of elements to retrieve.        Available collections: Attachments, Contacts, CustomFields, Invoices, Notes, Classification
     * @param order - The sort order for the results, in the [Searchlight order syntax](https://github.com/tspence/csharp-searchlight).
     * @param pageSize - The page size for results (default 200, maximum of 10,000)
     * @param pageNumber - The page number for results (default 0)
     */
    queryCompanies(filter, include, order, pageSize, pageNumber) {
        const url = `/api/v1/Companies/query`;
        const options = {
            params: {
                filter,
                include,
                order,
                pageSize,
                pageNumber,
            },
        };
        return this.client.request('get', url, options, null);
    }
    /**
     * Queries Customer Summaries for this account using the specified filtering, sorting, nested fetch, and pagination rules requested.
     *
     * More information on querying can be found on the [Searchlight Query Language](https://developer.lockstep.io/docs/querying-with-searchlight) page on the Lockstep Developer website.  The Customer Summary View represents a slightly different view of the data and includes some extra fields that might be useful. For more information, see the data format of the Customer Summary Model.  See [Vendors, Customers, and Companies](https://developer.lockstep.io/docs/companies-customers-and-vendors) for more information.
     *
     * @param filter - The filter for this query. See [Searchlight Query Language](https://developer.lockstep.io/docs/querying-with-searchlight)
     * @param include - To fetch additional data on this object, specify the list of elements to retrieve.        No collections are currently available but may be offered in the future
     * @param order - The sort order for the results, in the [Searchlight order syntax](https://github.com/tspence/csharp-searchlight).
     * @param pageSize - The page size for results (default 200, maximum of 10,000)
     * @param pageNumber - The page number for results (default 0)
     */
    queryCustomerSummary(filter, include, order, pageSize, pageNumber) {
        const url = `/api/v1/Companies/views/customer-summary`;
        const options = {
            params: {
                filter,
                include,
                order,
                pageSize,
                pageNumber,
            },
        };
        return this.client.request('get', url, options, null);
    }
    /**
     * Retrieves the Customer Details specified by this unique identifier, optionally including nested data sets.  The Customer Detail View represents a slightly different view of the data and includes some extra fields that might be useful. For more information, see the data format of the Customer Detail Model.  See [Vendors, Customers, and Companies](https://developer.lockstep.io/docs/companies-customers-and-vendors) for more information.
     *
     * @param id - The unique Lockstep Platform ID number of this Company; NOT the customer's ERP key
     */
    retrieveCustomerDetail(id) {
        const url = `/api/v1/Companies/views/customer-details/${id}`;
        return this.client.request('get', url, null, null);
    }
}
//# sourceMappingURL=CompaniesClient.js.map