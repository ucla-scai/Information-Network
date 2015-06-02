import java.util.*;
import java.io.*;

public class ValidCommunityMapping {

	public static void main(String args[]) throws IOException {

		if(args.length != 1 )
		{
			System.out.println("Improper No. of arguments! Need the file having communities !");
			System.exit(1);
		}

		String inputfile = args[0];

    		FileInputStream fis = new FileInputStream(new File(inputfile));
    		BufferedReader br = new BufferedReader(new InputStreamReader(fis));
            	String rec = null;

		boolean adv_found = false;
		boolean key_found = false;
		int cnt=0;
		String last_seen_community=null;
			
		while ((rec = br.readLine()) != null) 
            	{
			String val[] = rec.split("\\s+");

			if(rec.startsWith("["))
			{
				if(adv_found==true && key_found==true)
				{
					cnt++;
					System.out.println("From:"+last_seen_community + " To:"+cnt);
				}
				adv_found = false;	
				key_found = false;
				if(val[0].endsWith("]"))
					last_seen_community = val[0].replace("[","");
				else
					last_seen_community = val[1];
				last_seen_community = last_seen_community.replace("]","");
			}

			for(int i=0; i<val.length; i++)
			{
				if(adv_found==false && (val[i]).startsWith("A"))
					adv_found=true;	
				if(key_found==false && (val[i]).startsWith("K"))
					key_found=true;
			}
		}	
            	br.close();				
	}
}
