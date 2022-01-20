require 'lockstep_sdk/lockstep_api'
require 'json'

class Lockstep

  attr_accessor :api_key

  def initialize(api_key:)
    @api_key = api_key
  end

  def create_client
    env = 'sbx'
    client = LockstepSdk::LockstepApi.new(env)
    client.with_api_key(@api_key)
    if not client
      puts "ISSUE WITH CLIENT, NO API KEY OR WRONG ENVIRONMENT"
    else
      puts "CLIENT WAS CREATED SUCCESSFULLY: {client}"
      return client
    end
  end

end

lockstep_sdk = Lockstep.new(ENV['API_KEY'])
client = lockstep_sdk.create_client

status_results = client.status.ping()
puts "StatusResult: #{status_results}"

page_num = 0
count = 1

while page_num < 5
  invoices = client.invoices.query_invoices(
    filter: "invoiceDate GT 2021-01-10 AND invoiceDate LT 2021-03-10",
    include_param: "Company",
    order: "invoiceDate asc",
    pageSize: 100,
    pageNum: page_num)

  if len(invoices['records']) == 0
    break
  end

  invoices = JSON.parse(invoices)
  invoices['records'].each do |record|
    puts "Invoice: #{record['invoiceId']}"
    puts "Company name: #{record['company']['companyName']}"
    puts "Outstanding Balance: #{record['outstandingBalanceAmount']}"
  end

  page_num += 1
end