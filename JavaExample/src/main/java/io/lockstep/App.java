package io.lockstep;

import java.util.Map;

import io.lockstep.api.*;
import io.lockstep.api.clients.InvoicesClient;
import io.lockstep.api.models.FetchResult;
import io.lockstep.api.models.InvoiceModel;
import io.lockstep.api.models.LockstepResponse;
import io.lockstep.api.models.StatusModel;

public class App
{
    public static void main( String[] args )
    {
        
        Map<String, String> environment = System.getenv();
        String key = environment.getOrDefault("LOCKSTEPAPI_SBX", null);
        LockstepApi client = LockstepApi.withEnvironment("sbx");
        
        if (key != null) {
            client.withApiKey(key);
        }
        
        LockstepResponse<StatusModel> response = client.getStatusClient().ping();
        
        //check if LockstepAPI is connected
        System.out.println("Ping result: " + response.isSuccess());

        int pageNumber = 0;
        int count = 0;
        InvoicesClient invoiceClient = client.getInvoicesClient();
        InvoiceModel[] invoiceModelList;
        LockstepResponse<FetchResult<InvoiceModel>> invoices;
        do {
            
            invoices = invoiceClient.queryInvoices("invoiceDate > 2021-12-01", "Company", "invoiceDate asc", 100, pageNumber);
            FetchResult<InvoiceModel> fetch = invoices.getValue();
            invoiceModelList = fetch.getRecords();

            for (InvoiceModel currentInvoice : invoiceModelList) {
                System.out.println("Invoice " + count + ":" + currentInvoice.getInvoiceId());
                System.out.println("Company Name: " + currentInvoice.getCustomer().getCompanyName());
                System.out.println("Outstanding Balance: " + currentInvoice.getOutstandingBalanceAmount());
                count++;
            }
            pageNumber++;
           
        //repeat until there is an error or we have printed out every invoice
        } while (invoices.isSuccess() && invoiceModelList.length > 0);
    }
}
  
