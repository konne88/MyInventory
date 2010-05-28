
EXTRA_DIST =  Makefile

# Warning: This is an automatically generated file, do not edit!

srcdir=.
top_srcdir=..

include $(top_srcdir)/config.make

ifeq ($(CONFIG),DEBUG)
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG"
ASSEMBLY = bin/Debug/Libs.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

GDK_SHARP_DLL_CONFIG_SOURCE=gdk-sharp.dll.config
GLIB_SHARP_DLL_CONFIG_SOURCE=glib-sharp.dll.config
GTK_SHARP_DLL_CONFIG_SOURCE=gtk-sharp.dll.config
LIBGDKSHARPGLUE_2_SO_SOURCE=libgdksharpglue-2.so
LIBGLIBSHARPGLUE_2_SO_SOURCE=libglibsharpglue-2.so
LIBGTKSHARPGLUE_2_SO_SOURCE=libgtksharpglue-2.so
PANGO_SHARP_DLL_CONFIG_SOURCE=pango-sharp.dll.config
LIBS_DLL_MDB_SOURCE=bin/Debug/Libs.dll.mdb
LIBS_DLL_MDB=$(BUILD_DIR)/Libs.dll.mdb

endif

ifeq ($(CONFIG),RELEASE)
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize-
ASSEMBLY = bin/Release/Libs.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

GDK_SHARP_DLL_CONFIG_SOURCE=gdk-sharp.dll.config
GLIB_SHARP_DLL_CONFIG_SOURCE=glib-sharp.dll.config
GTK_SHARP_DLL_CONFIG_SOURCE=gtk-sharp.dll.config
LIBGDKSHARPGLUE_2_SO_SOURCE=libgdksharpglue-2.so
LIBGLIBSHARPGLUE_2_SO_SOURCE=libglibsharpglue-2.so
LIBGTKSHARPGLUE_2_SO_SOURCE=libgtksharpglue-2.so
PANGO_SHARP_DLL_CONFIG_SOURCE=pango-sharp.dll.config
LIBS_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(GDK_SHARP_DLL_CONFIG) \
	$(GLIB_SHARP_DLL_CONFIG) \
	$(GTK_SHARP_DLL_CONFIG) \
	$(LIBGDKSHARPGLUE_2_SO) \
	$(LIBGLIBSHARPGLUE_2_SO) \
	$(LIBGTKSHARPGLUE_2_SO) \
	$(PANGO_SHARP_DLL_CONFIG) \
	$(LIBS_DLL_MDB)  

LINUX_PKGCONFIG = \
	$(LIBS_PC)  


RESGEN=resgen2

GDK_SHARP_DLL_CONFIG = $(BUILD_DIR)/gdk-sharp.dll.config
GLIB_SHARP_DLL_CONFIG = $(BUILD_DIR)/glib-sharp.dll.config
GTK_SHARP_DLL_CONFIG = $(BUILD_DIR)/gtk-sharp.dll.config
LIBGDKSHARPGLUE_2_SO = $(BUILD_DIR)/libgdksharpglue-2.so
LIBGLIBSHARPGLUE_2_SO = $(BUILD_DIR)/libglibsharpglue-2.so
LIBGTKSHARPGLUE_2_SO = $(BUILD_DIR)/libgtksharpglue-2.so
PANGO_SHARP_DLL_CONFIG = $(BUILD_DIR)/pango-sharp.dll.config
LIBS_PC = $(BUILD_DIR)/libs.pc

FILES = \
	Dummy.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	gdk-sharp.dll.config \
	glib-sharp.dll.config \
	gtk-sharp.dll.config \
	libgdksharpglue-2.so \
	libglibsharpglue-2.so \
	libgtksharpglue-2.so \
	pango-sharp.dll.config \
	libs.pc.in 

REFERENCES =  \
	System

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

#Targets
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG)  $(top_srcdir)/config.make

include $(top_srcdir)/Makefile.include
#include $(srcdir)/custom-hooks.make



$(eval $(call emit-deploy-target,GDK_SHARP_DLL_CONFIG))
$(eval $(call emit-deploy-target,GLIB_SHARP_DLL_CONFIG))
$(eval $(call emit-deploy-target,GTK_SHARP_DLL_CONFIG))
$(eval $(call emit-deploy-target,LIBGDKSHARPGLUE_2_SO))
$(eval $(call emit-deploy-target,LIBGLIBSHARPGLUE_2_SO))
$(eval $(call emit-deploy-target,LIBGTKSHARPGLUE_2_SO))
$(eval $(call emit-deploy-target,PANGO_SHARP_DLL_CONFIG))
$(eval $(call emit-deploy-wrapper,LIBS_PC,libs.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'


$(ASSEMBLY_MDB): $(ASSEMBLY)
$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	make pre-all-local-hook prefix=$(prefix)
	mkdir -p $(shell dirname $(ASSEMBLY))
	make $(CONFIG)_BeforeBuild
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
	make $(CONFIG)_AfterBuild
	make post-all-local-hook prefix=$(prefix)

install-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-install-local-hook prefix=$(prefix)
	make install-satellite-assemblies prefix=$(prefix)
	mkdir -p '$(DESTDIR)$(libdir)/$(PACKAGE)'
	$(call cp,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(GDK_SHARP_DLL_CONFIG),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(GLIB_SHARP_DLL_CONFIG),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(GTK_SHARP_DLL_CONFIG),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(LIBGDKSHARPGLUE_2_SO),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(LIBGLIBSHARPGLUE_2_SO),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(LIBGTKSHARPGLUE_2_SO),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(PANGO_SHARP_DLL_CONFIG),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call cp,$(LIBS_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	mkdir -p '$(DESTDIR)$(libdir)/pkgconfig'
	$(call cp,$(LIBS_PC),$(DESTDIR)$(libdir)/pkgconfig)
	make post-install-local-hook prefix=$(prefix)

uninstall-local: $(ASSEMBLY) $(ASSEMBLY_MDB)
	make pre-uninstall-local-hook prefix=$(prefix)
	make uninstall-satellite-assemblies prefix=$(prefix)
	$(call rm,$(ASSEMBLY),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(ASSEMBLY_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(GDK_SHARP_DLL_CONFIG),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(GLIB_SHARP_DLL_CONFIG),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(GTK_SHARP_DLL_CONFIG),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(LIBGDKSHARPGLUE_2_SO),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(LIBGLIBSHARPGLUE_2_SO),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(LIBGTKSHARPGLUE_2_SO),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(PANGO_SHARP_DLL_CONFIG),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(LIBS_DLL_MDB),$(DESTDIR)$(libdir)/$(PACKAGE))
	$(call rm,$(LIBS_PC),$(DESTDIR)$(libdir)/pkgconfig)
	make post-uninstall-local-hook prefix=$(prefix)

install: install-local
uninstall: uninstall-local
clean: clean-local

include $(top_srcdir)/rules.make
