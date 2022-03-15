from lockstep import LockstepApi
import os
import csv


def main():
    apikey = os.environ.get('LOCKSTEP_API_SBX')
    env = 'sbx'
    client = LockstepApi(env, "Invoice Collection Report")
    client.with_api_key(apikey)

    # Here you can define your query for invoices. Currently, we are 
    # filtering on outstandingBalanceAmount > 0 and invoiceDate 
    # created before 2021. Also, to get additional information about
    # primary contact, I've included the Customer table. 
    page_size = 10
    page_number = 0
    before_year = 2022
    invoices = client.invoices.query_invoices(
        "outstandingBalanceAmount > 0 AND invoiceDate < " + str(before_year),
        "Customer",
        "",
        page_size,
        page_number
    )

    # Creating and populating CSV file 
    csv_file_name = 'InvoiceCollectionReport.csv'
    headers = ['ContactName', 'ContactEmail', 'InvoiceDate', 'InvoiceID', 'OutstandingBalance']
    writer = csv.writer(open(csv_file_name, 'w'), lineterminator='\n')
    writer.writerow(headers)

    for invoice in invoices.value.records:
        rows = [invoice.customerPrimaryContact.contactName,
                invoice.customerPrimaryContact.emailAddress,
                invoice.invoiceDate,
                invoice.invoiceId,
                invoice.outstandingBalanceAmount]
        writer.writerow(rows)


main()
