f="$1"
awk '{print $1}' $f | sort -u > adv.tmp	# 1st col - advertiser
awk '{print $2}' $f | sort -u > key.tmp # 2nd col - keyword

if [ -e igraph_vertices."$f" ]
then
	rm igraph_vertices."$f"
fi

while read adv
do
	echo "g.add_vertices(\"A"$adv"\")" >> igraph_vertices."$f"
done < adv.tmp

while read key
do
	echo "g.add_vertices(\"K"$key"\")" >> igraph_vertices."$f"
done < key.tmp

rm adv.tmp
rm key.tmp
