package io.lockstep.api;

import java.io.FileWriter;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import io.lockstep.api.clients.CompaniesClient;
import io.lockstep.api.models.CompanyModel;
import io.lockstep.api.models.FetchResult;
import io.lockstep.api.models.LockstepResponse;
import io.lockstep.api.models.StatusModel;

public class CompanyReport {

    private String fileName;

    public CompanyReport(String fileName) {
        this.fileName = fileName;
    }

    public void writeToFile() throws IOException{

        Map<String, String> environment = System.getenv();
        String key = environment.getOrDefault("LOCKSTEPAPI_SBX", null);
        LockstepApi client = LockstepApi.withEnvironment("sbx");
        
        //if ApiKey exists, use it
        if (key != null) {
            client.withApiKey(key);
        }
        
        LockstepResponse<StatusModel> response = client.getStatusClient().ping();
        
        //check if LockstepAPI is connected correctly
        System.out.println("Ping result: " + response.isSuccess());
        CompaniesClient companiesClient = client.getCompaniesClient();
    
        int pageNum = 0;

        List<ArrayList<String>> rows = new ArrayList<ArrayList<String>>();
        LockstepResponse<FetchResult<CompanyModel>> companies;
        CompanyModel[] companyModelList;

        do{
            companies = companiesClient.queryCompanies("", "", "CompanyName asc", 100, pageNum);
            FetchResult<CompanyModel> fetch = companies.getValue();
            companyModelList = fetch.getRecords();

            //write out data for each row
            for (CompanyModel currentModel : companyModelList) {
                ArrayList<String> row = new ArrayList<String>();
                row.add(currentModel.getCompanyName());
                row.add(currentModel.getPhoneNumber());
                row.add(currentModel.getApEmailAddress());
                rows.add(row);
            }
            pageNum++;
        //repeat until we are out of companies
        } while (companies.isSuccess() && companyModelList.length > 0);

        //write out the first row
        FileWriter csvWriter = new FileWriter(fileName);
        csvWriter.append("Name");
        csvWriter.append(",");
        csvWriter.append("Phone Number");
        csvWriter.append(",");
        csvWriter.append("Email Address");
        csvWriter.append("\n");

        //write out every company for each subsequent row
        for (ArrayList<String> currentRow : rows) {
            csvWriter.append(String.join(",", currentRow));
            csvWriter.append("\n");
        }

        csvWriter.flush();
        csvWriter.close();
    }

    public static void main(String[] args) throws IOException {
        new CompanyReport("companies.csv").writeToFile();
    }
}
