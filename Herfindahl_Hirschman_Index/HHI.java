import java.util.*;
import java.io.*;

/*
	Format for cnsm.dat: <Advertiser> <Keyword> <Keyword weight for this advertiser>
	Input File: Detected Communities, advertiser node starts with A, keyword starts with K 
		    Every community should contain the community id within []
*/

public class HHI {

	static Hashtable<String,String> adv_key_cnsm = new Hashtable<String,String>();
	static ArrayList<String> adv = new ArrayList<String>();
	static ArrayList<String> kyw = new ArrayList<String>();
	static int comm_no = 0;

	public static void update_cnsm_info() throws IOException {
		String filename = "cnsm.dat";
		FileInputStream fis = new FileInputStream(new File(filename));
    		BufferedReader br = new BufferedReader(new InputStreamReader(fis));
            	String rec = null;

		while ((rec = br.readLine()) != null) 
           	{
			String val[] = rec.split("\\s+");
			String hk = "A"+val[0]+"-"+"K"+val[1];
			adv_key_cnsm.put(hk,val[2]);	
		}	
            	br.close();
	}

	public static void calculateHHI() {
		double hhi_sum = 0;
		
		comm_no++;
		System.out.print("Community:"+comm_no+" ");

		for(int k=0; k<kyw.size(); k++)
		{
			double hhi_kyw = 0;
			for(int a=0; a<adv.size(); a++)
			{
				String adv_kyw = adv.get(a)+"-"+kyw.get(k);
				if(adv_key_cnsm.containsKey(adv_kyw))
				{
					double x = Double.parseDouble(adv_key_cnsm.get(adv_kyw));
					hhi_kyw += x*x;
				}
			}
			hhi_sum += hhi_kyw;
		}

		System.out.println("Adv:"+adv.size()+" Kyw:"+kyw.size()+" TotHHI:"+hhi_sum+" AvgHHI:"+(double)hhi_sum/kyw.size());
	}

	public static void main(String args[]) throws IOException {

		if(args.length !=1 )
		{
			System.out.println("Input file argument missing!");
			System.exit(1);
		}

		update_cnsm_info();

		String inputfile = args[0];

    		FileInputStream fis = new FileInputStream(new File(inputfile));
    		BufferedReader br = new BufferedReader(new InputStreamReader(fis));
            	String rec = null;
			
		while ((rec = br.readLine()) != null) 
            	{
			if(rec.startsWith("["))
			{
				if(adv.size()>0 && kyw.size()>0)
					calculateHHI();

				adv.clear();
				kyw.clear();
			}

			String val[] = rec.split("\\s+");
			
			for(int i=0; i<val.length; i++)
			{
				if((val[i]).startsWith("A"))
					adv.add(val[i]);
				else if((val[i]).startsWith("K"))
					kyw.add(val[i]);
			}
		}	
            	br.close();				
	}
}
