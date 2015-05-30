f="$1"

./gen_unw_graph.sh "$f"

echo " " >> unw_graph."$f"

cp unw_graph."$f" run_infomap_unw_"$f".py
cp unw_graph."$f" run_eigen_unw_"$f".py
cp unw_graph."$f" run_multilevel_unw_"$f".py

echo "print g.community_infomap()" >> run_infomap_unw_"$f".py
echo "print g.community_leading_eigenvector()" >> run_eigen_unw_"$f".py
echo "print g.community_multilevel()" >> run_multilevel_unw_"$f".py

python run_infomap_unw_"$f".py | sed 's/,//g' > unw_"$f".infomap
python run_eigen_unw_"$f".py | sed 's/,//g' > unw_"$f".eigen
python run_multilevel_unw_"$f".py | sed 's/,//g' > unw_"$f".multilevel

./clean.sh "$f"
