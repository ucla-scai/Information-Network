f="$1"

if [ -e unw_graph."$f" ]
then
	rm unw_graph."$f"
fi

echo "import igraph" >> unw_graph."$f"
echo "g = igraph.Graph()" >> unw_graph."$f"

./add_vertices.sh "$f"
echo " " >> unw_graph."$f"
cat igraph_vertices."$f" >> unw_graph."$f"

./add_unw_edges.sh "$f"
echo " " >> unw_graph."$f"
cat igraph_unw_edges."$f" >> unw_graph."$f"

echo " " >> unw_graph."$f"

