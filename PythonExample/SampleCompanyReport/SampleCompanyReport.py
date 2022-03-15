from lockstep import LockstepApi
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
    client = LockstepApi(env, "Company Report App")
    client.with_api_key(apikey)
    status_results = client.status.ping()
    if not status_results.success or not status_results.value or not status_results.value.loggedIn:
        print("Your API key is not valid.")
        print("Please set the environment variable LOCKSTEPAPI_SBX and try again.")
        exit()
    print(f"Logged in as {status_results.value.accountName} {status_results.value.userName}")
    return client



def main():
    yourApiKey = retrieveApiKey()
    client = createClient(yourApiKey)

    # Define pagination rules
    pageSize = 5
    pageNumber = 1
    
    # Here you can define your desired query
    companies = client.companies.query_companies(
        None,
        None,
        None,
        pageSize,
        pageNumber
    )

    # Here we are defining a list of headers to write to help with column description
    yourFileName = 'SampleCompanyReport.csv'
    headers = ['CompanyName', 'CompanyPhoneNumber', 'AREmailAddress', 'APEmailAddress']
    writer = csv.writer(open(yourFileName, 'w'), lineterminator='\n')
    writer.writerow(headers)

    # Print out all the companies in the result
    for company in companies.value.records:
        rows = [company.companyName, company.phoneNumber, company.arEmailAddress, company.apEmailAddress]
        writer.writerow(rows)


main()
