from lockstep.lockstep_api import LockstepApi
import os


def retrieve_api_key():
    API_KEY = os.environ.get('LOCKSTEP_API_SBX')
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

    page_num = 0
    count = 1

    while page_num < 5:
        invoices = client.invoices.query_invoices(
            "invoiceDate GT 2021-01-10 AND invoiceDate LT 2021-03-10",
            "Company",
            "invoiceDate asc",
            100,
            page_num)

        if len(invoices['records']) == 0:
            break

        for record in invoices['records']:
            print(f"Invoice {count}: {record['invoiceId']}")
            print(f"Company name: {record['companyId']}")
            print(f"Outstanding Balance: ${record['outstandingBalanceAmount']} \n")
            count += 1

        page_num += 1



if __name__ == '__main__':
    main()