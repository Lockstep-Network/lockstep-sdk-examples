package io.lockstep.api;

import java.io.FileWriter;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import io.lockstep.api.clients.InvoicesClient;
import io.lockstep.api.models.ContactModel;
import io.lockstep.api.models.FetchResult;
import io.lockstep.api.models.InvoiceModel;
import io.lockstep.api.models.LockstepResponse;
import io.lockstep.api.models.StatusModel;

public class CollectionsReport {
   public static void main(String[] args) throws IOException {

    Map<String, String> environment = System.getenv();
    String key = environment.getOrDefault("LOCKSTEPAPI_SBX", null);
    LockstepApi client = LockstepApi.withEnvironment("sbx");
    
    if (key != null) {
        client.withApiKey(key);
    }
    
    LockstepResponse<StatusModel> response = client.getStatusClient().ping();
    
    //check if LockstepAPI is connected
    System.out.println("Ping result: " + response.isSuccess());
    InvoicesClient invoicesClient = client.getInvoicesClient();

    int pageNum = 0;

    List<ArrayList<String>> rows = new ArrayList<ArrayList<String>>();
    LockstepResponse<FetchResult<InvoiceModel>> invoices;
    InvoiceModel[] invoiceModelList;

    do{
        invoices = invoicesClient.queryInvoices("invoiceDate < 2021-12-01 AND outstandingBalanceAmount > 0", "", "InvoiceDate asc", 100, pageNum);
        FetchResult<InvoiceModel> fetch = invoices.getValue();
        invoiceModelList = fetch.getRecords();

        //write out data for each row
        for (InvoiceModel currentModel : invoiceModelList) {
            ArrayList<String> currentRow = new ArrayList<String>();
            currentRow.add(currentModel.getInvoiceId());
            currentRow.add(currentModel.getInvoiceDate());
            ContactModel model = currentModel.getCustomerPrimaryContact();
            if (model != null) {
                currentRow.add(currentModel.getCustomerPrimaryContact().getContactName());
            }
            rows.add(currentRow);
        }

        pageNum++;
    //repeat until we are out of invoices
    } while (invoices.isSuccess() && invoiceModelList.length > 0);

    //write out the first row
    FileWriter csvWriter = new FileWriter("collections.csv");
    csvWriter.append("Invoice ID");
    csvWriter.append(",");
    csvWriter.append("Invoice Date");
    csvWriter.append(",");
    csvWriter.append("Primary Contact Person");
    csvWriter.append("\n");

    //write out every invoice for each row
    for (ArrayList<String> currentRow : rows) {
        csvWriter.append(String.join(",", currentRow));
        csvWriter.append("\n");
    }

     csvWriter.flush();
     csvWriter.close();
   } 
}
