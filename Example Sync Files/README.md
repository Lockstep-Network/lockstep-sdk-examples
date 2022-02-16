# Lockstep Example Data Files

We have published data definitions for the import models for each data type online, and we've also published example files in Microsoft Excel formats for many of them. Right click on any of the links below to download these example files.

For each data model, we have three definitions:
* The import data format, which is used during the import process.  Data you provide to Lockstep is published in this format.
* An example excel file demonstrating what data looks like in this import format.
* The Lockstep Platform API data model, which can then be used to interact with the data after it has been imported.

You can import your own data into the Lockstep Platform API by formatting it to match these example excel files and then calling the [Upload Sync File API](https://developer.lockstep.io/reference/post_api-v1-sync-zip).

| Import Data Model |	Example Excel file | Lockstep Platform Data Type |
|--|--|--|
| [CompanySyncModel](https://developer.lockstep.io/docs/companysyncmodel) | [Company.xlsx](https://github.com/Lockstep-Network/lockstep-sdk-examples/blob/main/Example%20Sync%20Files/company.xlsx?raw=true) | [CompanyModel](https://developer.lockstep.io/docs/companymodel) | 
| [ContactSyncModel](https://developer.lockstep.io/docs/contactsyncmodel) | [Contact.xlsx](https://github.com/Lockstep-Network/lockstep-sdk-examples/blob/main/Example%20Sync%20Files/contact.xlsx?raw=true) | [ContactModel](https://developer.lockstep.io/docs/contactmodel) | 
| [CreditMemoAppliedSyncModel](https://developer.lockstep.io/docs/creditmemoappliedsyncmodel) | [CreditMemoApplied.xlsx](https://github.com/Lockstep-Network/lockstep-sdk-examples/blob/main/Example%20Sync%20Files/creditmemoapplied.xlsx?raw=true) | [CreditMemoAppliedModel](https://developer.lockstep.io/docs/creditmemoappliedmodel) |
| [CustomFieldSyncModel](https://developer.lockstep.io/docs/customfieldsyncmodel) |  | [CustomFieldValueModel](https://developer.lockstep.io/docs/customfieldvaluemodel) |
| [InvoiceLineSyncModel](https://developer.lockstep.io/docs/invoicelinesyncmodel) |  | [InvoiceLineModel](https://developer.lockstep.io/docs/invoicelinemodel) | 
| [InvoiceSyncModel](https://developer.lockstep.io/docs/invoicesyncmodel) | [Invoice.xlsx](https://github.com/Lockstep-Network/lockstep-sdk-examples/blob/main/Example%20Sync%20Files/invoice.xlsx?raw=true) |[InvoiceModel](https://developer.lockstep.io/docs/invoicemodel) | 
| [PaymentAppliedSyncModel](https://developer.lockstep.io/docs/paymentappliedsyncmodel) | [PaymentApplied.xlsx](https://github.com/Lockstep-Network/lockstep-sdk-examples/blob/main/Example%20Sync%20Files/paymentapplied.xlsx?raw=true) | [PaymentAppliedModel](https://developer.lockstep.io/docs/paymentappliedmodel) | 
| [PaymentSyncModel](https://developer.lockstep.io/docs/paymentsyncmodel) | [Payment.xlsx](https://github.com/Lockstep-Network/lockstep-sdk-examples/blob/main/Example%20Sync%20Files/payment.xlsx?raw=true) | [PaymentModel](https://developer.lockstep.io/docs/paymentmodel) | 
