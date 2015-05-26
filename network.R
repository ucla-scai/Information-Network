library('igraph')
dat <- read.table('demo1.txt')
g <- graph.data.frame(dat,directed=FALSE)
pdf("network.pdf", width = 200, height = 200)
plot(g, layout=layout.fruchterman.reingold.grid, vertex.size=1, 
     edge.arrow.size=0.5,vertex.label=NA)
dev.off()

#layout=layout.fruchterman.reingold(g, niter=10000)