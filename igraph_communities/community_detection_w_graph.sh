f="$1"

./gen_w_graph.sh "$f"

echo " " >> w_graph."$f"

cp w_graph."$f" run_infomap_w_"$f".py
cp w_graph."$f" run_eigen_w_"$f".py
cp w_graph."$f" run_multilevel_w_"$f".py

echo "print g.community_infomap()" >> run_infomap_w_"$f".py
echo "print g.community_leading_eigenvector()" >> run_eigen_w_"$f".py
echo "print g.community_multilevel()" >> run_multilevel_w_"$f".py

python run_infomap_w_"$f".py | sed 's/,//g' > w_"$f".infomap
python run_eigen_w_"$f".py | sed 's/,//g' > w_"$f".eigen
python run_multilevel_w_"$f".py | sed 's/,//g' > w_"$f".multilevel

./clean.sh "$f"
