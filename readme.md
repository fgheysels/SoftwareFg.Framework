SoftwareFg.Framework is a hobby project that dates back to 2008.  At that time I was particulary interested in O/R mappers and I was playing around with NHibernate.
This project contains some infrastructure code I wrote at that time that allowed me to easily use and configure NHibernate.

At the same time, I was also checking out on Aspect Oriented Programming.  
I wanted to be able to create a class where some properties can be locked at runtime so that the value of those properties could not be changed once they were locked.  
To do that, the class has to implement a specific interface (ILockable) and the lockable properties have to be decorated with the Lockable-attribute.
The Lockable attribute is in fact an aspect created with Postsharp that executes some extra code, determining whether or not the property has been locked.

More information on how it works can be found in a [blog-article](http://fgheysels.blogspot.be/2008/08/locking-system-with-aspect-oriented.html) I wrote back in the days.

I've upgraded the used libraries (NHibernate and Postsharp) to a more recent version and modified the code accordingly.  
This means offcourse that the examples in the blog-article no longer match the code in this repository.
