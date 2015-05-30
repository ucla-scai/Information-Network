f="$1"

if [ -e igraph_vertices."$f" ]
then
	rm igraph_vertices."$f"
fi

if [ -e igraph_unw_edges."$f" ]
then
	rm igraph_unw_edges."$f"
fi

if [ -e unw_graph."$f" ]
then
	rm unw_graph."$f"
fi

if [ -e igraph_w_edges."$f" ]
then
	rm igraph_w_edges."$f"
fi

if [ -e w_graph."$f" ]
then
	rm w_graph."$f"
fi
