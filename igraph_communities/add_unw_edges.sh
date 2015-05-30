f="$1"
awk '{print $1,$2}' $f | sed 's/ /\",\"K/g' > edges.tmp

if [ -e igraph_unw_edges."$f" ]
then
	rm igraph_unw_edges."$f"
fi

while read e
do
	echo "g.add_edges([(\"A"$e"\")])" >> igraph_unw_edges."$f"
done < edges.tmp

rm edges.tmp
