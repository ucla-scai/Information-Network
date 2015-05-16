# ./run_corr.sh 20130113-accnt.dat

cat $1 | awk '{print $1,$5,$6}' | sed 's/_/ /g' | sort -k2 > tr_"$1"
javac ClickConsumptionRelation.java 
java ClickConsumptionRelation tr_"$1" | sort -k3 -r > corr_"$1"
