% Generated by roxygen2: do not edit by hand
% Please edit documentation in R/SensusR.R
\name{sensus.sync.from.aws.s3}
\alias{sensus.sync.from.aws.s3}
\title{Synchronizes data from Amazon S3 to a local path.}
\usage{
sensus.sync.from.aws.s3(s3.path, profile = "default",
  local.path = tempfile(), aws.path = "/usr/local/bin/aws",
  delete = FALSE, decompress = FALSE)
}
\arguments{
\item{s3.path}{Path within S3. This can be a prefix (partial path).}

\item{profile}{AWS credentials profile to use for authentication.}

\item{local.path}{Path to location on local machine.}

\item{aws.path}{Path to AWS client.}

\item{delete}{Whether or not to delete local files that are not present in the S3 path.}

\item{decompress}{Whether or not to decompress any gzip files after downloading them.}
}
\value{
Local path to location of downloaded data.
}
\description{
Synchronizes data from Amazon S3 to a local path.
}
