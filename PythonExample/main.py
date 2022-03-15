from lockstep.lockstep_api import LockstepApi

import lockstep
import os
import json

def retrieve_api_key():
    API_KEY = os.environ.get('LOCKSTEPAPI_SBX')
    if API_KEY is None:
        print('NO API KEY')
    else:
        print('VALID API KEY FOUND')
        return API_KEY


def create_client(apikey):
    env = 'sbx'
    client = LockstepApi(env, 'DEFAULT_APP_NAME')
    client.with_api_key(apikey)
    if not client:
        print("ISSUE WITH CLIENT, NO API KEY OR WRONG ENVIRONMENT")
    else:
        print(f"CLIENT WAS CREATED SUCCESSFULLY: {client}")
        return client


def main():
    API_KEY = retrieve_api_key()
    client = create_client(API_KEY)
    status_results = client.status.ping()
    print(f"StatusResult: {status_results}")
    if not status_results.success or not status_results.value or not status_results.value.loggedIn:
        print("Your API key is not valid.")
        print("Please set the environment variable LOCKSTEPAPI_SBX and try again.")
        exit()
    print(f"Logged in as {status_results.value.accountName} {status_results.value.userName}")

    page_num = 0
    count = 1

    while page_num < 5:
        invoices = client.invoices.query_invoices(
            "invoiceDate GT 2021-01-10 AND invoiceDate LT 2021-03-10",
            "Company",
            "invoiceDate asc",
            100,
            page_num)

        if len(invoices.value.records) == 0:
            break

        for invoice in invoices.value.records:
            print(f"Invoice {count}: {invoice.invoiceId}")
            print(f"Company name: {invoice.companyId}")
            print(f"Outstanding Balance: ${invoice.outstandingBalanceAmount} \n")
            count += 1

        page_num += 1



if __name__ == '__main__':
    main()