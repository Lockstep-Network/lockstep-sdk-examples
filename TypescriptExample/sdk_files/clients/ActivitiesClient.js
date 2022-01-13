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
export class ActivitiesClient {
    /**
     * Internal constructor for this client library
     */
    constructor(client) {
        this.client = client;
    }
    /**
     * Retrieves the Activity specified by this unique identifier, optionally including nested data sets.
     *
     * An Activity contains information about work being done on a specific accounting task. You can use Activities to track information about who has been assigned a specific task, the current status of the task, the name and description given for the particular task, the priority of the task, and any amounts collected, paid, or credited for the task.
     *
     * @param id - The unique Lockstep Platform ID number of this Activity
     * @param include - To fetch additional data on this object, specify the list of elements to retrieve.        Available collections: Attachments, CustomFields, and Notes
     */
    retrieveActivity(id, include) {
        const url = `/api/v1/Activities/${id}`;
        const options = {
            params: {
                include,
            },
        };
        return this.client.request('get', url, options, null);
    }
    /**
     * Updates an activity that matches the specified id with the requested information.
     *
     * The PATCH method allows you to change specific values on the object while leaving other values alone.  As input you should supply a list of field names and new values.  If you do not provide the name of a field, that field will remain unchanged.  This allows you to ensure that you are only updating the specific fields desired.
     *
     * An Activity contains information about work being done on a specific accounting task. You can use Activities to track information about who has been assigned a specific task, the current status of the task, the name and description given for the particular task, the priority of the task, and any amounts collected, paid, or credited for the task.
     *
     * @param id - The unique Lockstep Platform ID number of the Activity to update
     * @param body - A list of changes to apply to this Activity
     */
    updateActivity(id, body) {
        const url = `/api/v1/Activities/${id}`;
        return this.client.request('patch', url, null, body);
    }
    /**
     * Delete the Activity referred to by this unique identifier.
     *
     * An Activity contains information about work being done on a specific accounting task. You can use Activities to track information about who has been assigned a specific task, the current status of the task, the name and description given for the particular task, the priority of the task, and any amounts collected, paid, or credited for the task.
     *
     * @param id - The unique Lockstep Platform ID number of the Activity to delete
     */
    deleteActivity(id) {
        const url = `/api/v1/Activities/${id}`;
        return this.client.request('delete', url, null, null);
    }
    /**
     * Creates one or more activities from a given model.
     *
     * An Activity contains information about work being done on a specific accounting task. You can use Activities to track information about who has been assigned a specific task, the current status of the task, the name and description given for the particular task, the priority of the task, and any amounts collected, paid, or credited for the task.
     *
     * @param body - The Activities to create
     */
    createActivities(body) {
        const url = `/api/v1/Activities`;
        return this.client.request('post', url, null, body);
    }
    /**
     * Queries Activities for this account using the specified filtering, sorting, nested fetch, and pagination rules requested.
     *
     * More information on querying can be found on the [Searchlight Query Language](https://developer.lockstep.io/docs/querying-with-searchlight) page on the Lockstep Developer website.
     *
     * An Activity contains information about work being done on a specific accounting task. You can use Activities to track information about who has been assigned a specific task, the current status of the task, the name and description given for the particular task, the priority of the task, and any amounts collected, paid, or credited for the task.
     *
     * @param filter - The filter for this query. See [Searchlight Query Language](https://developer.lockstep.io/docs/querying-with-searchlight)
     * @param include - To fetch additional data on this object, specify the list of elements to retrieve.               Available collections: Attachments, CustomFields, and Notes
     * @param order - The sort order for this query. See See [Searchlight Query Language](https://developer.lockstep.io/docs/querying-with-searchlight)
     * @param pageSize - The page size for results (default 200). See [Searchlight Query Language](https://developer.lockstep.io/docs/querying-with-searchlight)
     * @param pageNumber - The page number for results (default 0). See [Searchlight Query Language](https://developer.lockstep.io/docs/querying-with-searchlight)
     */
    queryActivities(filter, include, order, pageSize, pageNumber) {
        const url = `/api/v1/Activities/query`;
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
}
//# sourceMappingURL=ActivitiesClient.js.map