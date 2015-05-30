f="$1"

if [ -e w_graph."$f" ]
then
	rm w_graph."$f"
fi

echo "import igraph" >> w_graph."$f"
echo "g = igraph.Graph()" >> w_graph."$f"

./add_vertices.sh "$f"
echo " " >> w_graph."$f"
cat igraph_vertices."$f" >> w_graph."$f"

./add_w_edges.sh "$f"
echo " " >> w_graph."$f"
cat igraph_w_edges."$f" >> w_graph."$f"

echo " " >> w_graph."$f"

