from lockstep.lockstep_api import LockstepApi
import os
import csv


def retrieveApiKey():
    """
    Retrieves the Api Key from local environment
    """
    APIKEY = os.environ.get('LOCKSTEP_API_SBX')
    if APIKEY is None:
        print('NO API KEY')
    else:
        print('VALID API KEY FOUND')
        return APIKEY


def createClient(apikey):
    """
    Takes in an Api Key and creates the Lockstep Client
    """
    env = 'sbx'
    client = LockstepApi(env)
    client.with_api_key(apikey)
    if not client:
        print("ISSUE WITH CLIENT, NO API KEY OR WRONG ENVIRONMENT")
    else:
        print(f"CLIENT WAS CREATED SUCCESSFULLY: {client}")
        return client


def main():
    # Retrieve Api Key
    yourApiKey = retrieveApiKey()

    # Create Client
    client = createClient(yourApiKey)


    """
    Here you can define your desired query
    (
        "Filter",
        "Include",
        "Order",
        "pageSize",
        "pageNumber"
    )
    """
    pageSize = 5
    pageNumber = 1
    records = client.companies.query_companies(
        "",
        "",
        "",
        pageSize,
        pageNumber
    )

    """
    the result variable access the initial key 'records' and
    returns the list of key : value pairs
    """
    result = records['records']

    """
    Here we are defining a list of headers to write to help with column description
    """
    yourFileName = 'SampleCompanyReport.csv'
    headers = ['CompanyName', 'CompanyPhoneNumber', 'AREmailAddress', 'APEmailAddress']
    writer = csv.writer(open(yourFileName, 'w'), lineterminator='\n')
    writer.writerow(headers)

    for item in result:
        rows = [item['companyName'], item['corpPhone'], item['arEmailAddress'], item['apEmailAddress']]
        writer.writerow(rows)


main()
