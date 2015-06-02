f="$1"
s="_$2"

awk '{print $1, $5}' "$f" | grep "$s " | grep -v " 0$" | sed "s/$s//g" | sed 's/ /,/g'  > click.tmp 

if [ -e adv_click.dat ]
then
	rm adv_click.dat
fi

unset x1 y1 sum1
while IFS=, read x1 y1; do
    ((sum1[$x1]+=y1)); done < click.tmp
for i in ${!sum1[@]}; do
    echo $i,${sum1[$i]} >> adv_click.dat
done

total_clicks=$(awk -F',' '{clsum+=$2}; END {print clsum}' adv_click.dat)
total_clicks_sq=$(bc -l <<< $total_clicks*$total_clicks)

vals=$(awk -F',' '{u+=$2*$2}; END {print u}' adv_click.dat)
hhi=$(bc -l <<< $vals/$total_clicks_sq)

total_cnsm=$(awk '{print $1, $6}' "$f" | grep "$s " | awk '{cnsum+=$2}; END {print cnsum}')

echo $hhi,$total_cnsm >> "$s".dat

rm click.tmp
rm adv_click.dat
