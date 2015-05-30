f="$1"
awk '{print $1,$2}' $f | sed "s/ /','K/g" > edges.tmp
awk '{print $3}' $f | sed "s/^/weight=/g" > wt.tmp
paste edges.tmp wt.tmp | sed "s/\t/',/g" > wt_edges.tmp

if [ -e igraph_w_edges."$f" ]
then
	rm igraph_w_edges."$f"
fi

while read e
do
	echo "g.add_edge('A"$e")" >> igraph_w_edges."$f"
done < wt_edges.tmp

rm edges.tmp
rm wt.tmp
rm wt_edges.tmp
