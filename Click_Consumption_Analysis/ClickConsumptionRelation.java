import java.util.*;
import java.io.*;

/* The input file is the advertiser account file after some data transformation.
   The input file format is as follows:
   <Advertiser Accnt> <Sector> <Clicks> <Consumption>
   Input file sorted by column 2 (i.e. sector)
*/
public class ClickConsumptionRelation {

	static Hashtable<String,String> adv_clck = new Hashtable<String,String>();
	static Hashtable<String,String> adv_cnsm = new Hashtable<String,String>();

	public static void analyzeData(String filename) throws IOException 
	{
		FileInputStream fis = new FileInputStream(new File(filename));
    		BufferedReader br = new BufferedReader(new InputStreamReader(fis));
            	
		String rec = null;
		String sector = "-1";

		while ((rec = br.readLine()) != null) 
           	{
			String val[] = rec.split("\\s+");

			if(!sector.equals(val[1]))
			{				
				correl(sector);	//print correlation for this sector
				sector = val[1];
				adv_clck.clear();
				adv_cnsm.clear();
			}

			String adv = val[0];
			double clck = Double.parseDouble(val[2]);
			double cnsm = Double.parseDouble(val[3]);

			if(clck == 0 && cnsm ==0)	continue;	//ignore records without clicks and consumption			

			if(adv_clck.containsKey(adv))	//this key is present in both the hashtables
			{
				clck += Double.parseDouble(adv_clck.get(adv));
				cnsm += Double.parseDouble(adv_cnsm.get(adv));
			}

			adv_clck.put(adv,""+clck);	
			adv_cnsm.put(adv,""+cnsm);	
		}	
            	br.close();
	}

	public static void correl(String sector) 
	{
		double meanX = 0.0, meanY = 0.0;
		int no_of_adv=0;
		Enumeration en1=adv_clck.keys();

		while (en1.hasMoreElements()) 
		{
			String adv = en1.nextElement().toString();
			no_of_adv++;
			meanX += Double.parseDouble(adv_clck.get(adv));
       			meanY += Double.parseDouble(adv_cnsm.get(adv));
   		}

		if(no_of_adv==0)
			return;

    		meanX /= no_of_adv;
    		meanY /= no_of_adv;

    		double sumXY = 0.0, sumX2 = 0.0, sumY2 = 0.0;
		Enumeration en2=adv_cnsm.keys();

		while (en2.hasMoreElements()) 
		{
			String adv = en2.nextElement().toString();
			double x = Double.parseDouble(adv_clck.get(adv));
			double y = Double.parseDouble(adv_cnsm.get(adv));
 			sumXY += ((x - meanX) * (y - meanY));
      			sumX2 += Math.pow(x - meanX, 2.0);
      			sumY2 += Math.pow(y - meanY, 2.0);
   		}

		double val = (sumXY / (Math.sqrt(sumX2) * Math.sqrt(sumY2))) ; 

		System.out.println("Sector:" + sector + " Valid_Adv:" + no_of_adv + " Correlation:"+val);  
	}

	public static void main(String args[]) throws IOException {
		analyzeData(args[0]);
	}
}
