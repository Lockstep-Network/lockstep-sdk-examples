# Invoice Iteration Ruby Example
Many types of products examine invoices for a customer and provide feedback on them. A typical product might analyze incoming invoices and add metadata like a **credit score** for each invoice. This tutorial explains how to iterate through invoices and examine them.

We use the [Query Invoices API](https://developer.lockstep.io/reference/get_api-v1-invoices-query) to retrieve a collection of invoices. To fetch a large number of invoices, we must use [filtering and pagination](https://developer.lockstep.io/docs/querying-with-searchlight). Here's how it works

## Step 1: Add LockstepSdk gem to Gemfile
First,  you'll need to add the LockstepSdk gem. You can do so by adding the gem.
```Gemfile
gem LockstepSdk
```

Then you'll be able to start on your ruby file and utilize the LockstepSdk gem.
```ruby
require 'lockstep_sdk/lockstep_api'
require 'json' 
# env can also be prd or your own url addres
env = 'sbx'
client = LockstepSdk::LockstepApi.new(env)
client.with_api_key({INSERT_API_KEY})
```
### Check if you are connected to the API
```ruby
status_results = client.status.ping()
puts status_results
```

## Step 2: Create API Query
In this example, we will first query for invoices using the invoice date. We will fetch all invoices dated between Janurary 10, 2021 and May 10, 2021. We will specify a page size of 100 which gives us a small number of invoices in each query.
```ruby
invoices = client.invoices.query_invoices(
            pageSize: 100, # number of items per page
            pageNumber: 0, # page number
            filter: "invoiceDate GT 2021-01-10 AND invoiceDate LT 2021-05-10", # filter query
            include_param: "Company", # includes extra fields
            order: "invoiceDate asc" # ordering
) 
```
The results from this API call will list the first 100 invoices, since you specified the pagSize to be 100. The results will also include a `totalCount` field which tells you exactly how many records that matched your filter:
``` json
{
  "records": [
    {
      "groupKey": "1c043d8f-ce7e-4cf6-aa8e-a08b0220d327",
      "invoiceId": "23c57f74-b643-47bf-a82b-c90984b1fc1b",
      "companyId": "dab105d3-8fa3-4c4d-990a-c00cac91a064",
      "customerId": "453060e9-393e-4584-a5f6-31d8581c3969",
      "erpKey": "58784FB2-DD9F-4CAB-B672-575922DBFE3F",
      "purchaseOrderCode": "07E9A",
      "referenceCode": "DEMOI000000010",
      ...
    }
  ],
  "totalCount": 1919,
  "pageSize": 100
}
```
This is "Page Zero" of your results. You can then make additional API calls by specifying ?pageNumber=1 and so on.

## Step 3: Iterate through query results
```ruby
invoices = JSON.parse(invoices)
invoices['records'].each do |record|
  puts "Invoice: #{record['invoiceId']}"
  puts "Company name: #{record['company']['companyName']}"
  puts "Outstanding Balance: #{record['outstandingBalanceAmount'])}"
end
```
Here we are accessing the records key value pairs. For every record we have record[Invoice ID], record[Company Name], and an record[outstanding balance amount].

## Step 3 Continued: Expected results
Your results should appear something similar to the invoice below
```bash
Invoice: 123456
Company Name: Your_Company_Name
Ooutstanding Balance: $ 0.00
```
